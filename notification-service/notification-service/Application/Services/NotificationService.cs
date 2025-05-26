using Application.DTO;
using Application.Interfaces;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{

    public class NotificationService : INotificationService
    {
        public List<NotificationsDTO> GetNotificationsByType(string userId, NotificationEntity notificationType) {

            // get data based on the userId and notificationType


            return new List<NotificationsDTO>();
        }
    }
}
