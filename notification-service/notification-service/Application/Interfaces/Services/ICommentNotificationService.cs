using Domain.Events;

namespace Application.Interfaces.Services
{
    public interface ICommentNotificationService
    {
        Task UpdatCommentListNotification(CommentEvent commentEvent);
        Task RemoveCommentListNotification(CommentEvent commentEvent);


    }
}
