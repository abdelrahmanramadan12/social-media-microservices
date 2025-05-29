using Application.DTO;
using Application.Interfaces;
using Domain.Enums;

namespace Application.Services
{

    public class NotificationService : INotificationService
    {
        public List<NotificationsDTO> GetNotificationsByType(string userId, NotificationEntity notificationType)
        {

            return new List<NotificationsDTO>();
        }

        public List<NotificationsDTO> UnreadNotifications(string userId, NotificationEntity notificationEntity)
        {
            // get unread notifications based on the userId and notificationEntity
            return new List<NotificationsDTO>();
        }
        public bool MarkNotificationsAsRead(string userId, string notificationEntity)
        {
            return false;
        }
        public bool MarkAllNotificationsAsRead(string userId)
        {             // Logic to mark all notifications as read for the user
            return true;
        }
        public bool MarkNotificationAsUnread(string userId, string notificationEntity)
        {
            // Logic to mark a specific notification as unread for the user
            return true;
        }
        public Task<List<NotificationEntity>> GetNotificationTypes(string userId)
        {
            // Logic to get notification types for the user
            return Task.FromResult(new List<NotificationEntity>());
        }
    }
}
