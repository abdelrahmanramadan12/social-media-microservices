using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using react_service.Application.Events;
using react_service.Application.Interfaces.Listeners;
using react_service.Application.Interfaces.Repositories;
using react_service.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Workers
{
    public class PostEventListner : IPostEventListner
    {
        private readonly ILogger<PostEventListner> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private readonly List<string> _queueNames;
        private IConnection? _connection;
        private IChannel? _channel;

        public PostEventListner(
            ILogger<PostEventListner> logger,
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _scopeFactory = scopeFactory;

            _hostname = _configuration["RabbitMQ:PostEvent:HostName"] ?? "localhost";
            _username = _configuration["RabbitMQ:PostEvent:UserName"] ?? "guest";
            _password = _configuration["RabbitMQ:PostEvent:Password"] ?? "guest";
            var queueNamesStr = _configuration["RabbitMQ:PostEvent:QueueName"] ?? "PostEventTest";
            _queueNames = queueNamesStr.Split(';').ToList();

            if (string.IsNullOrEmpty(_hostname) || string.IsNullOrEmpty(_username) || 
                string.IsNullOrEmpty(_password) || !_queueNames.Any())
            {
                throw new InvalidOperationException("RabbitMQ:PostEvent configuration is incomplete. Please check appsettings.json");
            }
        }

        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Initializing PostEventListner for queues: {QueueNames}", string.Join(", ", _queueNames));
                
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

                _logger.LogInformation("Successfully initialized PostEventListner for queues: {QueueNames}", string.Join(", ", _queueNames));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing PostEventListner");
                throw;
            }
        }

        public Task ListenAsync(CancellationToken cancellationToken)
        {
            if (_channel == null)
            {
                _logger.LogError("Consumer not initialized");
                throw new InvalidOperationException("Consumer not initialized.");
            }

            _logger.LogInformation("Starting to listen on queues: {QueueNames}", string.Join(", ", _queueNames));
            
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);
                    _logger.LogInformation("Received message from queue {QueueName}: {Message}", ea.RoutingKey, messageJson);
                    
                    using var scope = _scopeFactory.CreateScope();
                    var postRepository = scope.ServiceProvider.GetRequiredService<IPostRepository>();
                    
                    var postEvent = JsonSerializer.Deserialize<PostEvent>(messageJson);
                    if (postEvent != null)
                    {
                        _logger.LogInformation("Processing PostEvent of type: {EventType}", postEvent.EventType);
                        
                        switch (postEvent.EventType)
                        {
                            case EventType.Create:
                                var post = new Post
                                {
                                    PostId = postEvent.PostId,
                                    AuthorId = postEvent.AuthorId,
                                    IsDeleted = false
                                };
                                var result = await postRepository.AddPost(post);
                                _logger.LogInformation("Post creation result: {Result}", result);
                                break;

                            case EventType.Delete:
                                var deleteResult = await postRepository.DeletePost(postEvent.PostId);
                                _logger.LogInformation("Post deletion result: {Result}", deleteResult);
                                break;

                            default:
                                _logger.LogWarning("Unhandled event type: {EventType}", postEvent.EventType);
                                break;
                        }

                        await _channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    else
                    {
                        _logger.LogError("Failed to deserialize PostEvent");
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                }
            };

            foreach (var queueName in _queueNames)
            {
                _channel.BasicConsumeAsync(
                    queue: queueName,
                    autoAck: false,
                    consumer: consumer);
            }

            _logger.LogInformation("Successfully started consuming from queues: {QueueNames}", string.Join(", ", _queueNames));
            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            _logger.LogInformation("Disposing PostEventListner for queues: {QueueNames}", string.Join(", ", _queueNames));
            
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