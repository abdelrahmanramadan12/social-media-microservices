using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using react_service.Application.Interfaces.Publishers;
using react_service.Application.DTO.RabbitMQ;

namespace react_service.Application.Services
{
    public class ReactionPublisher : IReactionPublisher, IAsyncDisposable
    {
        private readonly ILogger<ReactionPublisher> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private readonly List<string> _queueNames;
        private IConnection? _connection;
        private IChannel? _channel;

        public ReactionPublisher(
            ILogger<ReactionPublisher> logger,
            IConfiguration configuration)
        {
            _logger = logger;
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

                _logger.LogInformation("RabbitMQ connection established successfully for queues: {QueueNames}", string.Join(", ", _queueNames));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing RabbitMQ connection");
                throw;
            }
        }

        public async Task PublishReactionAsync(PostReactionEventDTO reactionEvent)
        {
            try
            {
                if (_channel == null)
                {
                    _logger.LogError("RabbitMQ channel is not initialized");
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

                    _logger.LogInformation("Published reaction event to queue {QueueName}: {Message}", queueName, message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing reaction event");
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
                _logger.LogError(ex, "Error disposing RabbitMQ resources");
            }
        }
    }
}
