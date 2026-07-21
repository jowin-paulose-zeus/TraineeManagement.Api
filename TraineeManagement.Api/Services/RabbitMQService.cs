using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.Configuration;

namespace TraineeManagement.Api.Services
{
    public class RabbitMQService(IOptions<RabbitMQSettings> options) : IRabbitMQService
    {
        private readonly RabbitMQSettings _settings = options.Value;

        public Task PublishSubmissionAsync<T>(T message)
        {
            ConnectionFactory factory = new()
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                VirtualHost = _settings.VirtualHost,
                UserName = _settings.Username,
                Password = _settings.Password
            };

            using IConnection connection = factory.CreateConnection();
            Console.WriteLine("Connected to RabbitMQ Succesfully");

            using IModel channel = connection.CreateModel();

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

            string json = JsonSerializer.Serialize(message);

            byte[] body = Encoding.UTF8.GetBytes(json);

            IBasicProperties properties = channel.CreateBasicProperties();

            properties.Persistent = true;

            channel.BasicPublish(
                exchange: "",
                routingKey: _settings.QueueName,
                basicProperties: properties,
                body: body);

            return Task.CompletedTask;
        }
    }
}