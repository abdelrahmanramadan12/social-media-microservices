using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace notification_service.Controllers
{
    [Route("api/internal/[controller]")]
    [ApiController]
    public class NotificationController(INotificationService notificationService) : ControllerBase
    {
        private readonly INotificationService _notificationService = notificationService;

        [HttpGet]
        public IActionResult GetNotificationsByType([FromHeader(Name = "userId")] string userId, [FromQuery] NotificationEntity notificationType)
        {
            _notificationService.GetNotificationsByType(userId, notificationType);
            return Ok();
        }

        [HttpGet]
        public IActionResult GetUnreadNotifications([FromHeader(Name = "userId")] string userId, NotificationEntity notificationEntity)
        {
            var unreadNotifications = _notificationService.UnreadNotifications(userId, notificationEntity); // Example usage of the service
            return Ok(unreadNotifications);
        }

        [HttpGet]
        public IActionResult GetUnreadNotificationCount([FromHeader(Name = "userId")] string userId, NotificationEntity notificationEntity)
        {
            var unreadCount = _notificationService.UnreadNotifications(userId, notificationEntity).Count;
            return Ok(unreadCount);
        }

        [HttpPut]
        public IActionResult MarkNotificationAsRead([FromHeader(Name = "userId")] string userId, [FromQuery] string notificationId)
                                      => Ok(_notificationService.MarkNotificationsAsRead(userId, notificationId));


        [HttpPut]
        public IActionResult MarkAllNotificationsAsRead([FromHeader(Name = "userId")] string userId, [FromQuery] string notificationId)
                                      => Ok(_notificationService.MarkAllNotificationsAsRead(userId));

        [HttpPut]
        public IActionResult MarkNotificationAsUnread([FromHeader(Name = "userId")] string userId, [FromQuery] string notificationId)
                                             => Ok(_notificationService.MarkNotificationAsUnread(userId, notificationId));


        [HttpGet]
        public IActionResult GetNotificationTypes([FromHeader(Name = "userId")] string userId)
                                                     => Ok(_notificationService.GetNotificationTypes(userId));


    }
}
