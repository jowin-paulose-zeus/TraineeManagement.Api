using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.Configuration;

namespace TraineeManagement.Api.Services
{
    public class RabbitMQService : IRabbitMQService, IAsyncDisposable
    {
        private readonly RabbitMQSettings _settings;
        private readonly ConnectionFactory _factory;
        private IConnection? _connection;
        private readonly SemaphoreSlim _connectionLock = new(1, 1);

        public RabbitMQService(IOptions<RabbitMQSettings> options)
        {
            _settings = options.Value;
            _factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                VirtualHost = _settings.VirtualHost,
                UserName = _settings.Username,
                Password = _settings.Password
            };
        }

        private async Task<IConnection> GetConnectionAsync()
        {
            if (_connection != null) return _connection;

            await _connectionLock.WaitAsync();
            try
            {
                _connection ??= await _factory.CreateConnectionAsync();
                Console.WriteLine("Connected to RabbitMQ Successfully");
                return _connection;
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public async Task<bool> RabbitMQPublish<T>(T message)
        {
            try
            {
                var connection = await GetConnectionAsync();
                
                await using IChannel channel = await connection.CreateChannelAsync();

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
                    arguments: arguments);

                string json = JsonSerializer.Serialize(message);
                byte[] body = Encoding.UTF8.GetBytes(json);

                BasicProperties properties = new()
                {
                    Persistent = true,
                };

                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: _settings.QueueName,
                    mandatory: false,
                    basicProperties: properties,
                    body: body);

                return true;
            }
            catch (Exception ex)
            {
                // TODO: Replace with proper ILogger logging
                Console.WriteLine($"RabbitMQ Publish Failed: {ex.Message}");
                return false;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
            {
                await _connection.DisposeAsync();
            }
            _connectionLock.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
