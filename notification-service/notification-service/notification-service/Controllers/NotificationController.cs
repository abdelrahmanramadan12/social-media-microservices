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
        
        #region GetNotifications
        /// <summary>
        /// Retrieves all notifications for the specified user.
        /// </summary>
        /// <remarks>The user ID must be valid and correspond to an existing user in the system.  If the
        /// user ID is missing or invalid, the response may indicate an error.</remarks>
        /// <param name="userId">The unique identifier of the user whose notifications are to be retrieved.  This value must be provided in
        /// the request header.</param>
        /// <returns>An <see cref="IActionResult"/> containing a collection of notifications for the specified user. Returns an
        /// empty collection if no notifications are found.</returns>

        [HttpGet]
        public IActionResult GetAllNotifications([FromHeader(Name = "userId")] string userId)
        {
            var notifications = _notificationService.GetAllNotifications(userId); // Example usage of the service
            return Ok(notifications);
        }

        [HttpGet]
        public IActionResult GetFollowNotification([FromHeader(Name = "userId")] string userId)
        {
            var followNotifications = _notificationService.GetFollowNotification(userId); // Example usage of the service
            return Ok(followNotifications);
        }

        [HttpGet]
        public IActionResult GetReactionsNotification([FromHeader(Name = "userId")] string userId)
        {
            var likeNotifications = _notificationService.GetReactionNotification(userId); // Example usage of the service
            return Ok(likeNotifications);
        }

        [HttpGet]
        public IActionResult GetCommentsNotification([FromHeader(Name = "userId")] string userId)
        {
            var mentionNotifications = _notificationService.GetCommentNotification(userId); // Example usage of the service
            return Ok(mentionNotifications);
        }


        #endregion


        #region GetUnreadNotifications
        /// <summary>
        /// Retrieves all unread notifications for the specified user.
        /// </summary>
        /// <remarks>This method uses the <c>_notificationService</c> to fetch unread notifications.
        /// Ensure that the <paramref name="userId"/> corresponds to a valid user.</remarks>
        /// <param name="userId">The unique identifier of the user whose unread notifications are to be retrieved. This value must be
        /// provided in the request header.</param>
        /// <returns>An <see cref="IActionResult"/> containing a collection of unread notifications for the user. If no unread
        /// notifications exist, the collection will be empty.</returns>

        [HttpGet]
        public IActionResult GetAllUnreadNotifications([FromHeader(Name = "userId")] string userId)
        {
            var unreadNotifications = _notificationService.GetAllUnseenNotification(userId); // Example usage of the service
            return Ok(unreadNotifications);
        }

        [HttpGet]
        public IActionResult GetUnreadReactionsNotifications([FromHeader(Name = "userId")] string userId)
        {
            var unreadReactionsNotifications = _notificationService.GetUnreadReactionsNotifications(userId); // Example usage of the service
            return Ok(unreadReactionsNotifications);
        }

        [HttpGet]
        public IActionResult GetUnreadCommentNotifications([FromHeader(Name = "userId")] string userId)
        {
            var unreadCommentNotifications = _notificationService.GetUnreadCommentNotifications(userId); // Example usage of the service
            return Ok(unreadCommentNotifications);
        }

        [HttpGet]
        public IActionResult GetUnreadFollowedNotifications([FromHeader(Name = "userId")] string userId)
        {
            var unreadFollowedNotifications = _notificationService.GetUnreadFollowedNotifications(userId); // Example usage of the service
            return Ok(unreadFollowedNotifications);
        }

        [HttpGet]
        public IActionResult GetUnreadNotificationCount([FromHeader(Name = "userId")] string userId)
        {
            var unreadCount = _notificationService.GetAllUnseenNotification(userId).Count;
            return Ok(unreadCount);
        }

        #endregion

       
        #region MarkNotificationsAsRead
        [HttpPut]
        public IActionResult MarkFollowingNotificationAsRead([FromHeader(Name = "userId")] string userId, [FromQuery] string notificationId)
                                      => Ok(_notificationService.MarkFollowingNotificationAsRead(userId, notificationId));
        [HttpPut]
        public IActionResult MarkCommentNotificationAsRead([FromHeader(Name = "userId")] string userId, [FromQuery] string notificationId)
                                      => Ok(_notificationService.MarkCommentNotificationAsRead(userId, notificationId));

        [HttpPut]
        public IActionResult MarkReactionNotificationAsRead([FromHeader(Name = "userId")] string userId, [FromQuery] string notificationId)
                                    => Ok(_notificationService.MarkReactionNotificationAsRead(userId, notificationId));
        #endregion


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
