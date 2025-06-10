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
    public class FollowListenerService : IFollowListener, IAsyncDisposable
    {
        private readonly RabbitMqListenerSettings _settings;
        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection? _connection;
        private IChannel? _channel;

        public FollowListenerService(
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
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(
                queue:  _settings.QueueName,
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
                    var followEvent = JsonSerializer.Deserialize<FollowEvent>(message);

                    if (followEvent == null || string.IsNullOrEmpty(followEvent.FollowerId) || string.IsNullOrEmpty(followEvent.UserId))
                        return;

                    using var scope = _scopeFactory.CreateScope();
                    var followService = scope.ServiceProvider.GetRequiredService<IFollowNotificationService>();

                    if (followEvent.EventType == FollowEventType.FOLLOW)
                        await followService.UpdateFollowersListNotification(followEvent);
                    else
                        await followService.RemoveFollowerFromNotificationList(followEvent);

                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing follow message: {ex.Message}");
                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            _channel.BasicConsumeAsync(
                queue: "FollowQueue",
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
