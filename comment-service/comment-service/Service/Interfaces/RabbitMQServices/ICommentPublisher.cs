using Domain.Events;

namespace Service.Interfaces.RabbitMQServices
{
    public interface ICommentPublisher
    {
        Task PublishAsync(CommentEvent evt, CancellationToken ct = default);

    }
}
