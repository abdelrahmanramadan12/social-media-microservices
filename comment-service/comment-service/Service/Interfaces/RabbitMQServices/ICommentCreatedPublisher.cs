using Domain.Events;

namespace Service.Interfaces.RabbitMQServices
{
    public interface ICommentCreatedPublisher
    {
        Task PublishAsync(CommentCreatedEvent evt, CancellationToken ct = default);

    }
}
