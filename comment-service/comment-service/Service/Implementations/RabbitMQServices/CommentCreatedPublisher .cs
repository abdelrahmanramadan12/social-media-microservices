using Domain.Events;
using Microsoft.Extensions.Configuration;
using Service.Interfaces.RabbitMQServices;

namespace Service.Implementations.RabbitMQServices
{
    public class CommentCreatedPublisher : ICommentCreatedPublisher
    {
        private readonly IRabbitMqPublisher _bus;
        private readonly string _queueName;

        public CommentCreatedPublisher(IRabbitMqPublisher bus, IConfiguration config)
        {
            _bus = bus;
            _queueName = config["RabbitMQQueues:CommentCreated"];
        }
        public Task PublishAsync(CommentCreatedEvent evt, CancellationToken ct = default)
            => _bus.PublishAsync(evt, _queueName, ct);

    }
}
