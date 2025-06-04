using Service.Events;

namespace Service.Interfaces.RabbitMQServices
{
    public interface ICommentDeletedPublisher
    {
        Task PublishAsync(CommentDeletedEvent evt);
    }
}