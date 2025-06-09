using Application.Abstractions;
using Application.Events;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Application.Implementations
{
    public class FollowQueuePublisher : IQueuePublisher<FollowEvent>
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private string _userName;
        private string _password;
        private string _hostName;
        private int _port;
        private string _queueName;

        public FollowQueuePublisher(IConfiguration config)
        {
            _userName = config.GetSection("FollowMQ:UserName").Value!;
            _password = config.GetSection("FollowMQ:Password").Value!;
            _hostName = config.GetSection("FollowMQ:HostName").Value!;
            _queueName = config.GetSection("FollowMQ:QueueName").Value!;
            _port = Convert.ToInt32(config.GetSection("FollowMQ:Port").Value);
        }

        public async Task InitializeAsync()
        {
            var factory = new ConnectionFactory
            {
                //UserName = _userName,
                //Password = _password,
                HostName = _hostName,
                //Port = _port
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
        }

        public async Task PublishAsync(FollowEvent args)
        {
            if (_channel == null)
                throw new InvalidOperationException("Listener not initialized.");

            await _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var message = JsonSerializer.Serialize(args);

            var bin = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: _queueName,
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
