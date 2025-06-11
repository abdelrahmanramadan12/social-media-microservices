using Domain.IRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Service.Events;
using Service.Interfaces.RabbitMQServices;
using System.Text;
using System.Text.Json;

namespace Service.Implementations.RabbitMQServices
{
    public class CommentReactionListener : IQueueListener<CommentReactionEvent>
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly string _queueName;
        private readonly string _hostName;
        private readonly IServiceScopeFactory _scopeFactory;

        public CommentReactionListener(IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _hostName = configuration["RabbitQueues:HostName"] ?? "localhost";
            _queueName = configuration["RabbitQueues:CommentReactionQueue"] ?? "CommentReactionEventTest";
        }

        public async Task InitializeAsync()
        {
            var factory = new ConnectionFactory() { HostName = _hostName };
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
        }

        public async Task ListenAsync(CancellationToken cancellationToken)
        {
            await _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                try
                {
                    var reactionEvent = JsonSerializer.Deserialize<CommentReactionEvent>(message);
                    if (reactionEvent != null && !string.IsNullOrEmpty(reactionEvent.CommentId))
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var commentRepo = scope.ServiceProvider.GetRequiredService<ICommentRepository>();
                        if (reactionEvent.EventType == ReactionEventType.Like)
                        {
                            await commentRepo.IncrementReactionCountAsync(reactionEvent.CommentId);
                        }
                        else if (reactionEvent.EventType == ReactionEventType.Unlike)
                        {
                            await commentRepo.DecrementReactionCountAsync(reactionEvent.CommentId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CommentReactionListener] Failed to process reaction event: {ex.Message}\n{ex.StackTrace}");
                }
                await Task.CompletedTask;
            };

            await _channel.BasicConsumeAsync(
                queue: _queueName,
                autoAck: true,
                consumerTag: string.Empty,
                noLocal: false,
                exclusive: false,
                arguments: null,
                consumer: consumer
            );
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
                await _channel.DisposeAsync();
            if (_connection != null)
                await _connection.DisposeAsync();
        }
    }
}
