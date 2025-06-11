using Service.Events;

namespace Service.Interfaces.RabbitMQServices
{
    public interface ICommentCreatedPublisher
    {
        Task PublishAsync(CommentEvent evt);
    }
} 