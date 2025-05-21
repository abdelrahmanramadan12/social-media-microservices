using Domain.Events;
using Service.Interfaces.RabbitMQServices;

namespace Service.Implementations.RabbitMQServices
{
    public class CommentCreatedPublisher : ICommentCreatedPublisher
    {
        private const string QueueName = "commentCreatedQueue";
        private readonly IRabbitMqPublisher _bus;
        public CommentCreatedPublisher(IRabbitMqPublisher bus) => _bus = bus;

        public Task PublishAsync(CommentCreatedEvent evt, CancellationToken ct = default)
            => _bus.PublishAsync(evt, QueueName, ct);

    }
}
