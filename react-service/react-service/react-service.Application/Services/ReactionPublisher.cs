
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using react_service.Application.Interfaces.Publishers;
using react_service.Application.DTO.RabbitMQ;
using System.Text.Json;
using System.Text;

namespace react_service.Application.Services
{
    public class ReactionPublisher : IReactionPublisher, IAsyncDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private readonly List<string> _queueNames;
        private IConnection? _connection;
        private IChannel? _channel;

        public ReactionPublisher(IConfiguration configuration)
        {
            _configuration = configuration;

            // Get RabbitMQ configuration
            _hostname = _configuration["RabbitMQ:Reaction:HostName"] ?? "localhost";
            _username = _configuration["RabbitMQ:Reaction:UserName"] ?? "guest";
            _password = _configuration["RabbitMQ:Reaction:Password"] ?? "guest";
            var queueNamesStr = _configuration["RabbitMQ:Reaction:QueueName"] ?? "ReactionQueue";
            _queueNames = queueNamesStr.Split(';').ToList();

            InitializeRabbitMQ().GetAwaiter().GetResult();
        }

        private async Task InitializeRabbitMQ()
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

        public async Task PublishReactionAsync(PostReactionEventDTO reactionEvent)
        {
            try
            {
                if (_channel == null)
                {
                    return;
                }

                var message = JsonSerializer.Serialize(reactionEvent);
                var body = Encoding.UTF8.GetBytes(message);

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
    }
}
