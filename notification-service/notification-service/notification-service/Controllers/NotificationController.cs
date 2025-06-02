using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace notification_service.Controllers
{
    [Route("api/internal/[controller]")]
    [ApiController]

        

    public class NotificationController(INotificationService notificationService, IConnectionMultiplexer connectionMultiplexer) : ControllerBase
    {

        private readonly INotificationService _notificationService = notificationService;
        private readonly IDatabase _redisDb = connectionMultiplexer.GetDatabase();

        #region Test Redis
        [HttpPost("set")]
        public async Task<IActionResult> SetKey(string key, string value)
        {
            await _redisDb.StringSetAsync(key, value);
            return Ok($"Key '{key}' set successfully.");
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetKey(string key)
        {
            var value = await _redisDb.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                return NotFound($"Key '{key}' not found.");

            return Ok(value.ToString());
        }
        #endregion


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

        [HttpGet("AllNotifications")]
        public IActionResult GetAllNotifications([FromHeader(Name = "userId")] string userId)
        {
            var notifications = _notificationService.GetAllNotifications(userId); // Example usage of the service
            return Ok(notifications);
        }

        [HttpGet("FollowNotification")]
        public IActionResult GetFollowNotification([FromHeader(Name = "userId")] string userId)
        {
            var followNotifications = _notificationService.GetFollowNotification(userId); // Example usage of the service
            return Ok(followNotifications);
        }

        [HttpGet("CommentsNotification")]
        public IActionResult GetCommentsNotification([FromHeader(Name = "userId")] string userId)
        {
            var mentionNotifications = _notificationService.GetCommentNotification(userId); // Example usage of the service
            return Ok(mentionNotifications);
        }

        [HttpGet("ReactionsNotification")]
        public IActionResult GetReactionsNotification([FromHeader(Name = "userId")] string userId)
        {
            var likeNotifications = _notificationService.GetReactionNotification(userId); // Example usage of the service
            return Ok(likeNotifications);
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

        [HttpGet("AllUnreadNotifications")]
        public IActionResult GetAllUnreadNotifications([FromHeader(Name = "userId")] string userId)
        {
            var unreadNotifications = _notificationService.GetAllUnseenNotification(userId); // Example usage of the service
            return Ok(unreadNotifications);
        }

        [HttpGet("UnreadReactionsNotifications")]
        public IActionResult GetUnreadReactionsNotifications([FromHeader(Name = "userId")] string userId)
        {
            var unreadReactionsNotifications = _notificationService.GetUnreadReactionsNotifications(userId); // Example usage of the service
            return Ok(unreadReactionsNotifications);
        }

        [HttpGet("UnreadCommentNotifications")]
        public IActionResult GetUnreadCommentNotifications([FromHeader(Name = "userId")] string userId)
        {
            var unreadCommentNotifications = _notificationService.GetUnreadCommentNotifications(userId); // Example usage of the service
            return Ok(unreadCommentNotifications);
        }

        [HttpGet("UnreadFollowedNotifications")]
        public IActionResult GetUnreadFollowedNotifications([FromHeader(Name = "userId")] string userId)
        {
            var unreadFollowedNotifications = _notificationService.GetUnreadFollowedNotifications(userId); // Example usage of the service
            return Ok(unreadFollowedNotifications);
        }

        #endregion


        #region MarkNotificationsAsRead
        [HttpPost("mark-notifications-follow-as-read")]
        public async Task<IActionResult> MarkNotificationsFollowAsRead([FromHeader(Name = "userId")] string userId, [FromQuery] string followerId)
        {
            try
            {
               await  _notificationService.MarkNotificationsFollowAsRead(userId, followerId);


            }
            catch(Exception ex)
            {
                throw ex;

            }
            return Ok();
        }

        [HttpPost("mark-all-notifications-as-read")]
        public async Task<IActionResult> MarkAllNotificationsAsRead([FromHeader(Name = "userId")] string userId)
        {
            await _notificationService.MarkAllNotificationsAsRead(userId);
            return Ok();
        }

        [HttpPost("mark-notifications-reaction-comment-as-read")]
        public async Task<IActionResult> MarkNotificationsReactionCommentAsRead([FromHeader(Name = "userId")] string userId, [FromQuery] string reactionId)
        {
            await _notificationService.MarkNotificationsReactionCommentAsRead(userId, reactionId);
            return Ok();
        }

        [HttpPost("mark-notifications-reaction-post-as-read")]
        public async Task<IActionResult> MarkNotificationsReactionPostAsRead([FromHeader(Name = "userId")] string userId, [FromQuery] string reactionId)
        {
           await  _notificationService.MarkNotificationsReactionPostAsRead(userId, reactionId);
            return Ok();
        }

        #endregion


        [HttpGet("get-notifications-types")]
        public IActionResult GetNotificationTypes()
                                                => Ok(_notificationService.GetNotificationTypes());

    }
}

