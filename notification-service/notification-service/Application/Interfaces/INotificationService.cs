using Application.DTO;
using Domain.CacheEntities;
using Domain.CacheEntities.Comments;
using Domain.CacheEntities.Reactions;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Interfaces
{
    public interface INotificationService
    {
        List<NotificationsDTO> GetAllNotifications(string userId);
        List<NotificationsDTO> GetFollowNotification(string userId);
        List<NotificationsDTO> GetCommentNotification(string userId);
        List<NotificationsDTO> GetReactionNotification(string userId);
        Task<bool> MarkAllNotificationsAsRead(string userId);
        Task<List<NotificationEntity>> GetNotificationTypes();
        List<NotificationsDTO> GetUnreadReactionsNotifications(string userId);
        List<NotificationsDTO> GetUnreadCommentNotifications(string userId);
        List<NotificationsDTO> GetUnreadFollowedNotifications(string userId);
        List<NotificationsDTO> GetAllUnseenNotification(string userId);
        
        //Task<bool> MarkCommentNotificationAsRead(string userId, string notificationId);
        //Task<bool> MarkFollowingNotificationAsRead(string userId, string notificationId);
        //Task<bool> MarkReactionNotificationAsRead(string userId, string notificationId);
        
        public Task<bool> MarkNotificationsReactionCommentAsRead(string userId, string reactionId);
        public Task<bool> MarkNotificationsReactionPostAsRead(string userId, string reactionId);
        public Task<bool> MarkNotificationsCommentAsRead(string userId, string CommentId);
        public Task<bool> MarkNotificationsFollowAsRead(string userId, string userFollowedId);
    }
}