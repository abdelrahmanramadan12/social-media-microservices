using Domain.Events;
using Microsoft.Extensions.Configuration;
using Service.Interfaces.RabbitMQServices;

namespace Service.Implementations.RabbitMQServices
{
    public class CommentPublisher : ICommentPublisher
    {
        private readonly IRabbitMqPublisher _bus;
        private readonly string _queueName;

        public CommentPublisher(IRabbitMqPublisher bus, IConfiguration config)
        {
            _bus = bus;
            _queueName = config["RabbitMQQueues:CommentCreated"];
        }
        public Task PublishAsync(CommentEvent evt, CancellationToken ct = default)
            => _bus.PublishAsync(evt, _queueName, ct);

    }
}
