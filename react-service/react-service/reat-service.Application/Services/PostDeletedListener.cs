using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using reat_service.Application.Interfaces.Listeners;
using reat_service.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace reat_service.Application.Services
{
  
        public class PostDeletedListener : IPostDeletedListener
        {
            private IConnection? _connection;
            private IChannel? _channel;
            private string _postId;
            private string _queueName;
            private readonly IServiceScopeFactory _scopeFactory;
            private readonly IReactionPostService reactionPostService;
            private const string _hostName = "localhost";

           public PostDeletedListener(IConfiguration config, IServiceScopeFactory scopeFactory ,IReactionPostService reactionPostService)
           {
                _postId = config.GetSection("PostDeletedMQ:QueueName").Value;
                _scopeFactory = scopeFactory;
                 this.reactionPostService = reactionPostService;
           }

            public async Task InitializeAsync()
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostName
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
            }

            public async Task ListenAsync(CancellationToken _cancellationToken)
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

                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.ReceivedAsync += async (model, args) =>
                {
                    var messageJson = Encoding.UTF8.GetString(args.Body.ToArray());
                    var postId = JsonSerializer.Deserialize<string>(messageJson);                   
                    await reactionPostService.DeleteReactionsByPostId(postId);
                    
                };

                await _channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: consumer, cancellationToken: _cancellationToken);
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
