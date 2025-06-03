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
    public class ReactionListenerService(IOptions<RabbitMqListenerSettings> options, IServiceScopeFactory scopeFactory) : IAsyncDisposable
    {
        private readonly RabbitMqListenerSettings _settings = options.Value;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private IConnection? _connection;
        private IModel? _channel;

        public Task InitializeAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(_settings.QueueName, durable: true, exclusive: false, autoDelete: false);

            return Task.CompletedTask;
        }

        public async Task ListenAsync(CancellationToken cancellationToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                try
                {

                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var ReactionEvent = JsonSerializer.Deserialize<ReactionEvent>(message);
                    if (ReactionEvent == null)
                    {
                        Console.WriteLine("[ReactionQueue] Received an empty or invalid message.");
                        return;
                    }
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        // Call the service to handle the Follow Event
                        var ReactionService = scope.ServiceProvider.GetRequiredService<IReactionNotificationService>();
                        // Update the follow counter based on the event type
                        await ReactionService.UpdateReactionsListNotification(ReactionEvent);
                    }


                    Console.WriteLine($"[ReactionQueue] Message received: {message}");

                    _channel.BasicConsume(_settings.QueueName, true, consumer);
                }
                catch (Exception ex)
                {
                    _channel!.BasicNack(ea.DeliveryTag, false, requeue: false);
                    // Handle the exception
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }

            };
            //return Task.CompletedTask;
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