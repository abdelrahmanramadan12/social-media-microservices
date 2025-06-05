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
    public class FollowListenerService(IOptions<RabbitMqListenerSettings> options, IServiceScopeFactory scopeFactory) : IFollowListener
    {
        private readonly RabbitMqListenerSettings _settings = options.Value;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private IConnection? _connection;
        private IModel? _channel;

        public Task InitializeAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
                //_settings.HostName,
                //UserName = _settings.UserName,
                //Password = _settings.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            //_channel.QueueDeclare(_settings.QueueName, durable: true, exclusive: false, autoDelete: false);
            //_channel.QueueDeclare("FollowQueue", durable: true, exclusive: false, autoDelete: false);


            return Task.CompletedTask;
        }

        public void ListenAsync(CancellationToken cancellationToken)
        {
            _channel.QueueDeclare("FollowQueue", durable: true, exclusive: false, autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            _channel.BasicConsume("FollowQueue", autoAck: false, consumer);

            consumer.Received += async (model, ea) =>
            {
                Console.WriteLine("Message received.");
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var followEvent = JsonSerializer.Deserialize<FollowEvent>(message);

                    using var scope = _scopeFactory.CreateScope();
                    var followService = scope.ServiceProvider.GetRequiredService<IFollowNotificationService>();

                    if (followEvent == null || string.IsNullOrEmpty(followEvent.FollowerId) || string.IsNullOrEmpty(followEvent.UserId))
                        return;

                    if (followEvent.EventType == FollowEventType.FOLLOW)
                        await followService.UpdateFollowersListNotification(followEvent);
                    else
                        await followService.RemoveFollowerFromNotificationList(followEvent);

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex}");
                    _channel.BasicNack(ea.DeliveryTag, false, requeue: false);
                }
            };

            //_channel.BasicConsume(_settings.QueueName, true, consumer);
            //_channel.BasicConsume("FollowQueue", true, consumer);
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