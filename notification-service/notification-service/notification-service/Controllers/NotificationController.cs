using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace notification_service.Controllers
{
    [Route("api/public/[controller]")]
    [ApiController]
    public class NotificationController(INotificationService notificationService, IConnectionMultiplexer connectionMultiplexer) : BaseController
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
        [HttpGet("AllNotifications")]
        public async Task<IActionResult> GetAllNotifications([FromHeader(Name = "userId")] string userId, [FromHeader(Name = "next")] string next)
        {
            var response = await _notificationService.GetAllNotifications(userId, next);
            return HandlePaginatedResponse(response);
        }

        [HttpGet("FollowNotification")]
        public async Task<IActionResult> GetFollowNotification([FromHeader(Name = "userId")] string userId, [FromHeader(Name = "next")] string next)
        {
            var response = await _notificationService.GetFollowNotification(userId, next);
            return HandlePaginatedResponse(response);
        }

        [HttpGet("CommentsNotification")]
        public async Task<IActionResult> GetCommentsNotification([FromHeader(Name = "userId")] string userId, [FromHeader(Name = "next")] string next)
        {
            var response = await _notificationService.GetCommentNotification(userId, next);
            return HandlePaginatedResponse(response);
        }

        [HttpGet("ReactionsNotification")]
        public async Task<IActionResult> GetReactionsNotification([FromHeader(Name = "userId")] string userId, [FromHeader(Name = "next")] string next)
        {
            var response = await _notificationService.GetReactionNotification(userId, next);
            return HandlePaginatedResponse(response);
        }

        [HttpGet("MessageNotification")]
        public async Task<IActionResult> GetMessagesNotification([FromHeader(Name = "userId")] string userId, [FromHeader(Name = "next")] string next)
        {
            var response = await _notificationService.GetMessageNotifications(userId, next);
            return HandlePaginatedResponse(response);
        }
        #endregion

        #region GetUnreadNotifications
        /// <summary>
        /// Retrieves all unread notifications for the specified user.
        /// </summary>
        [HttpGet("AllUnreadNotifications")]
        public async Task<IActionResult> GetAllUnreadNotifications([FromHeader(Name = "userId")] string userId, [FromHeader(Name = "next")] string next)
        {
            var response = await _notificationService.GetAllUnseenNotification(userId, next);
            return HandlePaginatedResponse(response);
        }

        [HttpGet("UnreadReactionsNotifications")]
        public async Task<IActionResult> GetUnreadReactionsNotifications([FromHeader(Name = "userId")] string userId, [FromHeader(Name = "next")] string next)
        {
            var response = await _notificationService.GetUnreadReactionsNotifications(userId, next);
            return HandlePaginatedResponse(response);
        }

        [HttpGet("UnreadCommentNotifications")]
        public async Task<IActionResult> GetUnreadCommentNotifications([FromHeader(Name = "userId")] string userId, [FromHeader(Name = "next")] string next)
        {
            var response = await _notificationService.GetUnreadCommentNotifications(userId, next);
            return HandlePaginatedResponse(response);
        }

        [HttpGet("UnreadFollowedNotifications")]
        public async Task<IActionResult> GetUnreadFollowedNotifications([FromHeader(Name = "userId")] string userId, [FromHeader(Name = "next")] string next)
        {
            var response = await _notificationService.GetUnreadFollowedNotifications(userId, next);
            return HandlePaginatedResponse(response);
        }

        [HttpGet("UnreadMessageNotifications")]
        public async Task<IActionResult> GetUnreadMessageNotifications([FromHeader(Name = "userId")] string userId, [FromHeader(Name = "next")] string next)
        {
            var response = await _notificationService.GetUnreadMessageNotifications(userId, next);
            return HandlePaginatedResponse(response);
        }
        #endregion

        #region MarkNotificationsAsRead
        [HttpPost("mark-notifications-follow-as-read")]
        public async Task<IActionResult> MarkNotificationsFollowAsRead(
            [FromHeader(Name = "userId")] string userId,
            [FromQuery] string followerId)
        {
            var response = await _notificationService.MarkNotificationsFollowAsRead(userId, followerId);
            return HandleResponse(response);
        }

        [HttpPost("mark-all-notifications-as-read")]
        public async Task<IActionResult> MarkAllNotificationsAsRead([FromHeader(Name = "userId")] string userId)
        {
            var response = await _notificationService.MarkAllNotificationsAsRead(userId);
            return HandleResponse(response);
        }

        [HttpPost("mark-notifications-reaction-comment-as-read")]
        public async Task<IActionResult> MarkNotificationsReactionCommentAsRead(
            [FromHeader(Name = "userId")] string userId,
            [FromQuery] string reactionId)
        {
            var response = await _notificationService.MarkNotificationsReactionCommentAsRead(userId, reactionId);
            return HandleResponse(response);
        }

        [HttpPost("mark-notifications-reaction-post-as-read")]
        public async Task<IActionResult> MarkNotificationsReactionPostAsRead(
            [FromHeader(Name = "userId")] string userId,
            [FromQuery] string reactionId)
        {
            var response = await _notificationService.MarkNotificationsReactionPostAsRead(userId, reactionId);
            return HandleResponse(response);
        }

        [HttpPost("mark-notifications-message-as-read")]
        public async Task<IActionResult> MarkNotificationsMessageAsRead(
            [FromHeader(Name = "userId")] string userId,
            [FromQuery] string messageId)
        {
            var response = await _notificationService.MarkNotificationsMessagesAsRead(userId, messageId);
            return HandleResponse(response);
        }
        #endregion

        [HttpGet("get-notifications-types")]
        public async Task<IActionResult> GetNotificationTypes()
        {
            var response = await _notificationService.GetNotificationTypes();
            return HandleResponse(response);
        }
    }
}