
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using react_service.Domain.Events;
using react_service.Domain.interfaces;
using react_service.Domain.RabbitSetting;
using System.Text;
using System.Text.Json;

namespace Application.Implementations
{
    public class ReactionQueuePublisher : IQueuePublisher<ReactionEvent>
    {
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitSetting RabbitMQ { get; }

        public ReactionQueuePublisher(RabbitSetting rabbitMQ)
        {
            RabbitMQ = rabbitMQ;
        }

        public async Task InitializeAsync()
        {
            var factory = new ConnectionFactory
            {
               UserName = RabbitMQ.UserName,
                Password = RabbitMQ.Password,
                HostName = RabbitMQ.HostName,
                Port = RabbitMQ.Port,
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
        }

        public async Task PublishAsync(ReactionEvent args)
        {
            if (_channel == null)
                throw new InvalidOperationException("Listener not initialized.");

            await _channel.QueueDeclareAsync(
                queue: RabbitMQ.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var message = JsonSerializer.Serialize(args);

            var bin = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: RabbitMQ.QueueName,
                mandatory: true,
                basicProperties: new BasicProperties
                {
                    Persistent = true
                },
                body: bin);
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
                await _channel.CloseAsync();

            if (_connection != null)
                await _connection.CloseAsync();
        }

       
    }
}