using Application.DTO;
using Domain.Enums;

namespace Application.Interfaces
{
    public interface INotificationService
    {
      public List<NotificationsDTO>  GetNotificationsByType(string userId, NotificationEntity notificationType);

    }
}