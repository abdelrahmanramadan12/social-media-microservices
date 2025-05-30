using Application.DTO;
using Domain.Enums;

namespace Application.Interfaces
{
    public interface INotificationService
    {
        List<NotificationsDTO> GetNotificationsByType(string userId, NotificationEntity notificationType);
        List<NotificationsDTO> UnreadNotifications(string userId, NotificationEntity notificationEntity);

        Task<List<NotificationEntity>> GetNotificationTypes();
        public Task<bool> MarkAllNotificationsAsRead(string userId);
        public Task<bool> MarkNotificationsReactionCommentAsRead(string userId, string reactionId);
        public Task<bool> MarkNotificationsReactionPostAsRead(string userId, string reactionId);
        public Task<bool> MarkNotificationsCommentAsRead(string userId, string CommentId);
        public Task<bool> MarkNotificationsFollowAsRead(string userId, string userFollowedId);
    }
}