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
    public class PostEventPublisher : IAsyncDisposable
    {
        private readonly ILogger<PostEventPublisher> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private readonly string _queueName;
        private IConnection? _connection;
        private IChannel? _channel;

        public PostEventPublisher(
            ILogger<PostEventPublisher> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            _hostname = _configuration["RabbitMQ:PostEvent:HostName"] ?? "localhost";
            _username = _configuration["RabbitMQ:PostEvent:UserName"] ?? "guest";
            _password = _configuration["RabbitMQ:PostEvent:Password"] ?? "guest";
            _queueName = _configuration["RabbitMQ:PostEvent:QueueName"] ?? "PostEventTest";

            if (string.IsNullOrEmpty(_hostname) || string.IsNullOrEmpty(_username) || 
                string.IsNullOrEmpty(_password) || string.IsNullOrEmpty(_queueName))
            {
                throw new InvalidOperationException("RabbitMQ:PostEvent configuration is incomplete. Please check appsettings.json");
            }
        }

        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Initializing PostEventPublisher for queue: {QueueName}", _queueName);
                
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

                _logger.LogInformation("Successfully initialized PostEventPublisher for queue: {QueueName}", _queueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing PostEventPublisher");
                throw;
            }
        }

        public async Task PublishPostEventAsync(PostEvent postEvent)
        {
            if (_channel == null)
            {
                _logger.LogError("Publisher not initialized");
                throw new InvalidOperationException("Publisher not initialized.");
            }

            try
            {
                var messageJson = JsonSerializer.Serialize(postEvent);
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

                _logger.LogInformation("Published post event to queue {QueueName}: {Message}", _queueName, messageJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing post event");
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            _logger.LogInformation("Disposing PostEventPublisher for queue: {QueueName}", _queueName);
            
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
                _logger.LogError(ex, "Error disposing RabbitMQ resources");
            }
        }
    }
} 