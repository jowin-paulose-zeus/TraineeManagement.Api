using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TraineeManagement.Data.Contracts;
using TraineeManagement.Worker.Configuration;
using TraineeManagement.Worker.Interfaces;
using TraineeManagement.Data.Models;
using TraineeManagement.Data.Enums;
using TraineeManagement.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace TraineeManagement.Worker;

public class Worker(
    ILogger<Worker> logger,
    IOptions<RabbitMQSettings> options,
    IServiceProvider serviceProvider) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly RabbitMQSettings _settings = options.Value;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private IConnection? connection;
    private IChannel? channel;
    private const int MaxRetryAttempts = 3;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ConnectionFactory factory = new()
        {
            HostName = _settings.Host,
            Port = _settings.Port,
            VirtualHost = _settings.VirtualHost,
            UserName = _settings.Username,
            Password = _settings.Password
        };

        connection = await factory.CreateConnectionAsync(stoppingToken);
        channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync(
            exchange: _settings.DeadLetterExchange,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: stoppingToken);

            Dictionary<string, object?> arguments = new()
            {
                { "x-dead-letter-exchange", _settings.DeadLetterExchange },
                { "x-dead-letter-routing-key", _settings.DeadLetterQueue }
            };

        await channel.QueueDeclareAsync(
            queue: _settings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: arguments,
            cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: _settings.DeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        await channel.QueueBindAsync(
            queue: _settings.DeadLetterQueue,
            exchange: _settings.DeadLetterExchange,
            routingKey: _settings.DeadLetterQueue,
            cancellationToken: stoppingToken);

        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: stoppingToken);

        AsyncEventingBasicConsumer consumer = new(channel);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            string json = Encoding.UTF8.GetString(body);

            SubmissionProcessingRequest? message = JsonSerializer.Deserialize<SubmissionProcessingRequest>(json);

            if (message is null)
            {
                _logger.LogWarning("Received an invalid message.");

                await channel.BasicRejectAsync(
                    deliveryTag: ea.DeliveryTag,
                    requeue: false,
                    cancellationToken: stoppingToken);

                return;
            }

            using IServiceScope? scope = _serviceProvider.CreateScope();
            ISubmissionProcessorService? processor = scope.ServiceProvider.GetRequiredService<ISubmissionProcessorService>();

            try
            {
                await processor.ProcessAsync(message, stoppingToken);

                await channel.BasicAckAsync(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);

                _logger.LogInformation("Successfully processed SubmissionId {SubmissionId}", message.SubmissionId);
            }
            catch (Exception ex)
            {
                using IServiceScope? retryScope = _serviceProvider.CreateScope();
                TraineeDbContext? context = retryScope.ServiceProvider.GetRequiredService<TraineeDbContext>();

                ProcessingJob? job = await context.ProcessingJobs
                    .FirstOrDefaultAsync(j => j.MessageId == message.MessageId, stoppingToken);

                if (job is null)
                {
                    _logger.LogError("ProcessingJob not found for MessageId {MessageId}", message.MessageId);

                    await channel.BasicNackAsync(
                        ea.DeliveryTag,
                        multiple: false,
                        requeue: true,
                        cancellationToken: stoppingToken);

                    return;
                }

                job.Attempts++;

                if (job.Attempts >= MaxRetryAttempts)
                {
                    job.Status = JobStatus.Failed;
                    job.CompletedAt = DateTime.UtcNow;
                    job.ErrorSummary = ex.Message;

                    await context.SaveChangesAsync(stoppingToken);

                    _logger.LogError(ex, "SubmissionId {SubmissionId} exceeded max retry attempts.", message.SubmissionId);

                    await channel.BasicNackAsync(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false,
                        requeue: false,
                        cancellationToken: stoppingToken); 
                }
                else
                {
                    await context.SaveChangesAsync(stoppingToken);

                    _logger.LogWarning(ex, "Retry {Attempt}/{MaxRetryAttempts} for SubmissionId {SubmissionId}", 
                        job.Attempts, MaxRetryAttempts, message.SubmissionId);

                    await channel.BasicNackAsync(
                        deliveryTag: ea.DeliveryTag,
                        multiple: false,
                        requeue: true,
                        cancellationToken: stoppingToken);
                }
            }
        };

        await channel.BasicConsumeAsync(
            queue: _settings.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation("Worker started. Waiting for messages on queue {QueueName}.", _settings.QueueName);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        _logger.LogInformation("Worker stopped.");
    }
}
