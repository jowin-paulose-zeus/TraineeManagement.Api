using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TraineeManagement.Contracts.Contracts;
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

        using IConnection? connection = factory.CreateConnection();
        using IModel? channel = connection.CreateModel();


        channel.ExchangeDeclare(
            exchange: _settings.DeadLetterExchange,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false);

        Dictionary<string, object> arguments = new()
        {
            { "x-dead-letter-exchange", _settings.DeadLetterExchange },
            { "x-dead-letter-routing-key", _settings.DeadLetterQueue }
        };

        channel.QueueDeclare(
            queue: _settings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: arguments);

        channel.QueueDeclare(
            queue: _settings.DeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        channel.QueueBind(
            queue: _settings.DeadLetterQueue,
            exchange: _settings.DeadLetterExchange,
            routingKey: _settings.DeadLetterQueue);

        channel.BasicQos(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false);

        EventingBasicConsumer? consumer = new(channel);

        consumer.Received += async (sender, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            string json = Encoding.UTF8.GetString(body);

            SubmissionProcessingRequest? message = JsonSerializer.Deserialize<SubmissionProcessingRequest>(json);

            if (message == null)
            {
                _logger.LogWarning("Received an invalid message.");

                channel.BasicNack(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false,
                    requeue: false);

                return;
            }

            using IServiceScope? scope = _serviceProvider.CreateScope();

            ISubmissionProcessorService? processor = scope.ServiceProvider
                .GetRequiredService<ISubmissionProcessorService>();

            try
            {
                await processor.ProcessAsync(message, stoppingToken);

                channel.BasicAck(
                    deliveryTag: ea.DeliveryTag,
                    multiple: false);

                _logger.LogInformation(
                    "Successfully processed SubmissionId {SubmissionId}",
                    message.SubmissionId);
            }

            catch (Exception ex)
            {
                using IServiceScope? retryScope = _serviceProvider.CreateScope();

                TraineeDbContext? context = retryScope.ServiceProvider
                    .GetRequiredService<TraineeDbContext>();

                ProcessingJob? job = await context.ProcessingJobs
                    .FirstOrDefaultAsync(
                        j => j.MessageId == message.MessageId,
                        stoppingToken);

                if (job == null)
                {
                    _logger.LogError(
                        "ProcessingJob not found for MessageId {MessageId}",
                        message.MessageId);

                    channel.BasicNack(
                        ea.DeliveryTag,
                        false,
                        false);

                    return;
                }

                if (job.Attempts >= MaxRetryAttempts)
                {
                    job.Status = JobStatus.Failed;
                    job.CompletedAt = DateTime.UtcNow;
                    job.ErrorSummary = ex.Message;

                    await context.SaveChangesAsync(stoppingToken);

                    _logger.LogError(
                        ex,
                        "SubmissionId {SubmissionId} exceeded the maximum retry attempts.",
                        message.SubmissionId);

                    channel.BasicNack(
                        ea.DeliveryTag,
                        false,
                        false);
                }
                else
                {
                    _logger.LogWarning(
                        ex,
                        "Retry {Attempt}/{MaxRetryAttempts} for SubmissionId {SubmissionId}",
                        job.Attempts,
                        MaxRetryAttempts,
                        message.SubmissionId);

                    channel.BasicNack(
                        ea.DeliveryTag,
                        false,
                        true);
                }
            }

        };

        channel.BasicConsume(
            queue: _settings.QueueName,
                autoAck: false,
                consumer: consumer);

        _logger.LogInformation(
            "Worker started. Waiting for messages on queue {QueueName}.",
            _settings.QueueName);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        _logger.LogInformation("Worker stopped.");
    }
}