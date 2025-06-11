using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using react_service.Application.Events;
using react_service.Application.Interfaces.Publishers;
using System.Text;
using System.Text.Json;

namespace Workers
{
    public class CommentReactionPublisher : IQueuePublisher<CommentReactionEvent>
    {
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private readonly int _port;
        private readonly List<string> _queueNames;
        private IConnection? _connection;
        private IChannel? _channel;

        public CommentReactionPublisher(IConfiguration configuration)
        {
            _username = configuration.GetSection("RabbitQueues:Username").Value!;
            _password = configuration.GetSection("RabbitQueues:Password").Value!;
            _hostname = configuration.GetSection("RabbitQueues:HostName").Value!;
            _queueNames = configuration.GetSection("RabbitQueues:CommentReactionQueue").GetChildren().Select(c => c.Value).ToList()!;
            _port = Convert.ToInt32(configuration.GetSection("RabbitQueues:Port").Value);

            if (string.IsNullOrEmpty(_hostname) || string.IsNullOrEmpty(_username) ||
                string.IsNullOrEmpty(_password) || !_queueNames.Any())
            {
                throw new InvalidOperationException("RabbitMQ:Reaction configuration is incomplete. Please check appsettings.json");
            }
        }

        public async Task InitializeAsync()
        {
            try
            {

                var factory = new ConnectionFactory
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                foreach (var queueName in _queueNames)
                {
                    await _channel.QueueDeclareAsync(
                        queue: queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {

            try
            {
                if (_channel != null)
                {
                    await _channel.CloseAsync();
                }
                if (_connection != null)
                {
                    await _connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public async Task PublishAsync(CommentReactionEvent args)
        {
            if (_channel == null)
            {
                throw new InvalidOperationException("Publisher not initialized.");
            }

            try
            {
                var messageJson = JsonSerializer.Serialize(args);
                var body = Encoding.UTF8.GetBytes(messageJson);

                foreach (var queueName in _queueNames)
                {
                    await _channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: queueName,
                    mandatory: true,
                    basicProperties: new BasicProperties
                    {
                        Persistent = true
                    },
                    body: body);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}