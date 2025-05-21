using Domain.Events;
using Service.Interfaces.RabbitMQServices;

namespace Service.Implementations.RabbitMQServices
{
    public class CommentDeletedPublisher: ICommentDeletedPublisher
    {
        private const string QueueName = "commentDeletedQueue";
        private readonly IRabbitMqPublisher _bus;
        public CommentDeletedPublisher(IRabbitMqPublisher bus) => _bus = bus;

        public Task PublishAsync(CommentDeletedEvent evt, CancellationToken ct = default)
            => _bus.PublishAsync(evt, QueueName, ct);
    }
}
