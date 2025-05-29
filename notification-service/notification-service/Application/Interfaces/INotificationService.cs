using Application.DTO;
using Domain.Enums;

namespace Application.Interfaces
{
    public interface INotificationService
    {
        List<NotificationsDTO> GetNotificationsByType(string userId, NotificationEntity notificationType);
        List<NotificationsDTO> UnreadNotifications(string userId, NotificationEntity notificationEntity);
        bool MarkNotificationsAsRead(string userId, string notificationEntity);
        bool MarkAllNotificationsAsRead(string userId);
        bool MarkNotificationAsUnread(string userId, string notificationEntity);
        Task<List<NotificationEntity>> GetNotificationTypes(string userId);
    }
}