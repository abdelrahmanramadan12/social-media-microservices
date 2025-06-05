using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using react_service.Application.Interfaces.Listeners;
using react_service.Application.Interfaces.Services;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace react_service.Application.Services
{
    public class PostDeletedListener : IPostDeletedListener, IAsyncDisposable
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private readonly string _queueName;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IReactionPostService _reactionPostService;

        public PostDeletedListener(
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory,
            IReactionPostService reactionPostService)
        {
            _hostname = configuration["RabbitMQ:Listener:HostName"];
            _username = configuration["RabbitMQ:Listener:UserName"];
            _password = configuration["RabbitMQ:Listener:Password"];
            _queueName = configuration["RabbitMQ:Listener:QueueName"];
            _scopeFactory = scopeFactory;
            _reactionPostService = reactionPostService;
        }

        public async Task InitializeAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostname,
                UserName = _username,
                Password = _password
            };

            _connection =await  factory.CreateConnectionAsync();
            _channel = await  _connection.CreateChannelAsync();

           await _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

        }

        public Task ListenAsync(CancellationToken cancellationToken)
        {
            if (_channel == null)
                throw new InvalidOperationException("Listener not initialized.");

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);

                try
                {
                    var postId = JsonSerializer.Deserialize<string>(messageJson);
                    if (!string.IsNullOrEmpty(postId))
                    {
                        await _reactionPostService.DeleteReactionsByPostId(postId);
                    }
                }
                catch (Exception ex)
                {
                    // TODO: log exception
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            };

            _channel.BasicConsumeAsync(
                queue: _queueName,
                autoAck: true,
                consumer: consumer);

            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            _channel?.CloseAsync();
            _connection?.CloseAsync();

            _channel?.Dispose();
            _connection?.Dispose();

            return ValueTask.CompletedTask;
        }
    }
}
