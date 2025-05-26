using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using reat_service.Application.Interfaces.Listeners;
using reat_service.Application.Interfaces.Services;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace reat_service.Application.Services
{
    public class PostDeletedListener : IPostDeletedListener, IAsyncDisposable
    {
        private IConnection? _connection;
        private IModel? _channel;
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

        public Task InitializeAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostname,
                UserName = _username,
                Password = _password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            return Task.CompletedTask;
        }

        public Task ListenAsync(CancellationToken cancellationToken)
        {
            if (_channel == null)
                throw new InvalidOperationException("Listener not initialized.");

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
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

            _channel.BasicConsume(
                queue: _queueName,
                autoAck: true,
                consumer: consumer);

            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            _channel?.Close();
            _connection?.Close();

            _channel?.Dispose();
            _connection?.Dispose();

            return ValueTask.CompletedTask;
        }
    }
}
