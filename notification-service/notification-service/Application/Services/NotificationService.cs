using Application.DTO;
using Application.Interfaces;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
    }
}
