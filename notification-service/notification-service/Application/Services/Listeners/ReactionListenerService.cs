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
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.Listeners
{
    public class ReactionListenerService : IReactionListener, IAsyncDisposable
    {
        private readonly RabbitMqListenerSettings _settings;
        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection? _connection;
        private IChannel? _channel;

        public ReactionListenerService(
            IOptions<RabbitMqListenerSettings> options,
            IServiceScopeFactory scopeFactory)
        {
            _settings = options.Value;
            _scopeFactory = scopeFactory;
        }

        public async Task InitializeAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password,
                Port =_settings.Port,
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
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var reactionEvent = JsonSerializer.Deserialize<ReactionEvent>(message);
                    if (reactionEvent == null)
                    {
                        Console.WriteLine("[ReactionQueue] Received an invalid message.");
                        return;
                    }

                    using var scope = _scopeFactory.CreateScope();
                    var reactionService = scope.ServiceProvider.GetRequiredService<IReactionNotificationService>();

                    await reactionService.UpdateReactionsListNotification(reactionEvent);

                    Console.WriteLine($"[ReactionQueue] Message processed: {message}");

                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ReactionQueue] Error processing message: {ex.Message}");
                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            _channel.BasicConsumeAsync(
                queue: _settings.QueueName,
                autoAck: false,
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
