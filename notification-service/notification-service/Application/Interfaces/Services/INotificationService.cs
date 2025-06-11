using Application.DTO;
using Domain.CacheEntities;
using Domain.CacheEntities.Comments;
using Domain.CacheEntities.Reactions;
using Domain.Enums;
using ThirdParty.Json.LitJson;

namespace Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<PaginationResponseWrapper<List<NotificationsDTO>>> GetAllNotifications(string userId , string next );
        Task<PaginationResponseWrapper<List<NotificationsDTO>>> GetFollowNotification(string userId , string next);
        Task<PaginationResponseWrapper<List<NotificationsDTO>>> GetCommentNotification(string userId , string next);
        Task<PaginationResponseWrapper<List<NotificationsDTO>>> GetReactionNotification(string userId , string next);
        Task<PaginationResponseWrapper<List<NotificationsDTO>>> GetMessageNotifications(string userId ,string next);
        
        Task<ResponseWrapper<bool>> MarkAllNotificationsAsRead(string userId);
        Task<ResponseWrapper<List<NotificationEntity>>> GetNotificationTypes();
        Task<PaginationResponseWrapper<List<NotificationsDTO>>> GetUnreadMessageNotifications(string userId,string next);
        Task<PaginationResponseWrapper<List<NotificationsDTO>>> GetUnreadReactionsNotifications(string userId,string next);
        Task<PaginationResponseWrapper<List<NotificationsDTO>>> GetUnreadFollowedNotifications(string userId ,string next );
        Task<PaginationResponseWrapper<List<NotificationsDTO>>> GetUnreadCommentNotifications(string userId, string next );
        Task<PaginationResponseWrapper<List<NotificationsDTO>>> GetAllUnseenNotification(string userId ,string next );

        //Task<bool> MarkCommentNotificationAsRead(string userId, string notificationId);
        //Task<bool> MarkFollowingNotificationAsRead(string userId, string notificationId);
        //Task<bool> MarkReactionNotificationAsRead(string userId, string notificationId);

       Task<ResponseWrapper<bool>> MarkNotificationsReactionCommentAsRead(string userId, string reactionId);
       Task<ResponseWrapper<bool>>MarkNotificationsReactionPostAsRead(string userId, string reactionId);
        Task<ResponseWrapper<bool>> MarkNotificationsCommentAsRead(string userId, string CommentId);
        Task<ResponseWrapper<bool>> MarkNotificationsFollowAsRead(string userId, string userFollowedId);
        Task<ResponseWrapper<bool>> MarkNotificationsMessagesAsRead(string userId, string MessageId);

    }
}