using Application.Interfaces.Listeners;
using Application.Interfaces.Services;
using Domain.Events;
using Domain.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Application.Services.Listeners
{
    public class CommentListener(
        IOptions<RabbitMqListenerSettings> options,
        IServiceScopeFactory scopeFactory) : ICommentListener
    {
        private readonly RabbitMqListenerSettings _settings = options.Value;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private IConnection? _connection;
        private IChannel? _channel;

        public async Task InitializeAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(
                queue: _settings.QueueName,
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
                    var commentEvent = JsonSerializer.Deserialize<CommentEvent>(messageJson);

                    if (commentEvent == null || string.IsNullOrEmpty(commentEvent.Id))
                    {
                        Console.WriteLine("Invalid CommentEvent received.");
                        return;
                    }

                    using var scope = _scopeFactory.CreateScope();
                    var commentService = scope.ServiceProvider.GetRequiredService<ICommentNotificationService>();

                    if (commentEvent.CommentType == CommentType.ADDED)
                        await commentService.UpdatCommentListNotification(commentEvent);
                    else
                        await commentService.RemoveCommentListNotification(commentEvent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing comment message: {ex.Message}");
                }
            };

            _channel.BasicConsumeAsync(
                queue: _settings.QueueName,
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
