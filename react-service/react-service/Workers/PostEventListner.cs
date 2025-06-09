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
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private readonly List<string> _queueNames;
        private IConnection? _connection;
        private IChannel? _channel;

        public PostEventListner(IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
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

        public Task ListenAsync(CancellationToken cancellationToken)
        {
            if (_channel == null)
            {
                throw new InvalidOperationException("Consumer not initialized.");
            }

            
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);
                    
                    using var scope = _scopeFactory.CreateScope();
                    var postRepository = scope.ServiceProvider.GetRequiredService<IPostRepository>();
                    
                    var postEvent = JsonSerializer.Deserialize<PostEvent>(messageJson);
                    if (postEvent != null)
                    {
                        
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
                                break;

                            case EventType.Delete:
                                var deleteResult = await postRepository.DeletePost(postEvent.PostId);
                                break;

                            default:
                                break;
                        }

                        await _channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    else
                    {
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                    }
                }
                catch (Exception ex)
                {
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

            return Task.CompletedTask;
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