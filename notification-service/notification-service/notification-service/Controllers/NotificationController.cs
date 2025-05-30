using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

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


        [HttpPost("mark-all-notifications-as-read")]
        public IActionResult MarkAllNotificationsAsRead([FromHeader(Name = "userId")] string userId)
        {
            _notificationService.MarkAllNotificationsAsRead(userId);
            return Ok();
        }
        [HttpPost("mark-notifications-reaction-comment-as-read")]
        public IActionResult MarkNotificationsReactionCommentAsRead([FromHeader(Name = "userId")] string userId, [FromQuery] string reactionId)
        {
            _notificationService.MarkNotificationsReactionCommentAsRead(userId, reactionId);
            return Ok();
        }
        [HttpPost("mark-notifications-reaction-post-as-read")]
        public IActionResult MarkNotificationsReactionPostAsRead([FromHeader(Name = "userId")] string userId, [FromQuery] string  reactionId)
        {
            _notificationService.MarkNotificationsReactionPostAsRead(userId,  reactionId);
            return Ok();
        }
        [HttpPost("mark-notifications-comment-as-read")]

        public IActionResult MarkNotificationsCommentAsRead([FromHeader(Name = "userId")] string userId, [FromQuery] string commentId)
        {
            _notificationService.MarkNotificationsCommentAsRead(userId, commentId);
            return Ok();
        }
        [HttpPost("mark-notifications-follow-as-read")]
        public IActionResult MarkNotificationsFollowAsRead([FromHeader(Name = "userId")] string userId, [FromQuery] string followerId)
        {
            _notificationService.MarkNotificationsFollowAsRead(userId, followerId);
            return Ok();
        }

        [HttpGet]
        public IActionResult GetNotificationTypes()
                                                     => Ok(_notificationService.GetNotificationTypes());


    }
}
