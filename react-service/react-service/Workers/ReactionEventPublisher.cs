using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using react_service.Application.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Workers
{
    public class ReactionEventPublisher : IAsyncDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private readonly string _queueName;
        private IConnection? _connection;
        private IChannel? _channel;

        public ReactionEventPublisher(IConfiguration configuration)
        {
            _configuration = configuration;

            _hostname = _configuration["RabbitMQ:Reaction:HostName"] ?? "localhost";
            _username = _configuration["RabbitMQ:Reaction:UserName"] ?? "guest";
            _password = _configuration["RabbitMQ:Reaction:Password"] ?? "guest";
            _queueName = _configuration["RabbitMQ:Reaction:QueueName"] ?? "ReactionQueue";

            if (string.IsNullOrEmpty(_hostname) || string.IsNullOrEmpty(_username) || 
                string.IsNullOrEmpty(_password) || string.IsNullOrEmpty(_queueName))
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

                await _channel.QueueDeclareAsync(
                    queue: _queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task PublishReactionEventAsync(ReactionEvent reactionEvent)
        {
            if (_channel == null)
            {
                throw new InvalidOperationException("Publisher not initialized.");
            }

            try
            {
                var messageJson = JsonSerializer.Serialize(reactionEvent);
                var body = Encoding.UTF8.GetBytes(messageJson);

                await _channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: _queueName,
                    mandatory: true,
                    basicProperties: new BasicProperties
                    {
                        Persistent = true
                    },
                    body: body);

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