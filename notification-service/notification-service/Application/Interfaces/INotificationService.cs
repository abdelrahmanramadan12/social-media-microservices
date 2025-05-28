using Application.DTO;
using Domain.Enums;

namespace Application.Interfaces
{
    public interface INotificationService
    {
        List<NotificationsDTO> GetNotificationsByType(string userId, NotificationEntity notificationType);
        List<NotificationsDTO> UnreadNotifications(string userId, NotificationEntity notificationEntity);
        bool MarkNotificationsAsRead(string userId, string notificationEntity);

    }
}