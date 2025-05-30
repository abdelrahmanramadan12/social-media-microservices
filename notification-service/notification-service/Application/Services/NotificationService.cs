using Application.DTO;
using Application.Interfaces;
using Domain.CacheEntities;
using Domain.CacheEntities.Comments;
using Domain.CacheEntities.Reactions;
using Domain.Enums;
using Domain.Interfaces;
using System.Threading.Tasks;

namespace Application.Services
{
    public class NotificationService(IUnitOfWork unitOfWork1) : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork1;

        #region GetNotifications
        public List<NotificationsDTO> GetAllNotifications(string userId)
        {
            List<NotificationsDTO> notificationDto = [];

            notificationDto.AddRange(GetFollowNotification(userId));
            notificationDto.AddRange(GetCommentNotification(userId));
            notificationDto.AddRange(GetReactionNotification(userId));
            if (notificationDto.Count == 0)
                return [];

            return notificationDto;
        }
        public List<NotificationsDTO> GetFollowNotification(string userId)
        {
            List<NotificationsDTO> notificationDto = [];

            var NotificationBasedOnType = _unitOfWork.CacheRepository<CachedFollowed>()
                                                                                  .GetAsync(userId);
            if (NotificationBasedOnType == null)
                return [];
            var FollowedNotifications = NotificationBasedOnType.Result?.Followers;
            if (FollowedNotifications == null || FollowedNotifications.Count == 0)
                return [];

            notificationDto = [.. FollowedNotifications.Select(x => new NotificationsDTO
                {
                    SourceUserImageUrl = x.ProfileImageUrls,
                    IsRead=x.Seen,
                    CreatedTime= DateTime.Now,
                    EntityId = x.UsersId, // Assuming UsersId is the ID of the user who followed
                    EntityName = NotificationEntity.Follow,
                    NotificatoinPreview = $"{x.UserNames} started following you.",
                    SourceUsername= x.UserNames // Assuming UserNames is the name of the user who followed

                })];
            return notificationDto;
        }
        public List<NotificationsDTO> GetCommentNotification(string userId)
        {
            List<NotificationsDTO> notificationDto = [];

            var NotificationBasedOnType = _unitOfWork.CacheRepository<CachedComments>()
                                                                                   .GetAsync(userId);
            if (NotificationBasedOnType == null)
                return [];
            var CommentNotifications = NotificationBasedOnType.Result?.CommnetDetails;
            if (CommentNotifications == null || CommentNotifications.Count == 0)
                return [];
            notificationDto = [.. CommentNotifications.Select(x => new NotificationsDTO
                {
                    SourceUserImageUrl = x.User.ProfileImageUrls,
                    IsRead=x.User.Seen,
                    CreatedTime= DateTime.Now,
                    EntityId = x.CommentId, // Assuming EntityId is the ID of the comment entity
                    EntityName = NotificationEntity.Comment,
                    NotificatoinPreview = $"{x.User.UserNames} commented on your post.",
                    SourceUsername= x.User.UserNames // Assuming UserNames is the name of the user who commented
                })];
            return notificationDto;
        }
        public List<NotificationsDTO> GetReactionNotification(string userId)
        {
            List<NotificationsDTO> notificationDto = [];

            var NotificationBasedOnType = _unitOfWork.CacheRepository<CachedReactions>()
                                                                                  .GetAsync(userId);
            if (NotificationBasedOnType == null)
                return [];
            var ReactionOnPostsNotifications = NotificationBasedOnType.Result?.ReactionsOnPosts;
            if (ReactionOnPostsNotifications == null || ReactionOnPostsNotifications.Count == 0)
                return [];

            notificationDto = [.. ReactionOnPostsNotifications.Select(x => new NotificationsDTO
                {
                    SourceUserImageUrl = x.User.ProfileImageUrls,
                    IsRead=x.User.Seen,
                    CreatedTime= DateTime.Now,
                    EntityId = x.PostId, // Assuming EntityId is the ID of the reaction entity
                    EntityName = NotificationEntity.React,
                    NotificatoinPreview = $"{x.User.UserNames} reacted to your post.",
                    SourceUsername= x.User.UserNames // Assuming UserNames is the name of the user who reacted
                })];

            var ReactionOnCommentNotifications = NotificationBasedOnType.Result?.ReactionsOnComments;
            var ReactionOnCommentDto = ReactionOnCommentNotifications?.Select(x => new NotificationsDTO
            {
                SourceUserImageUrl = x.User.ProfileImageUrls,
                IsRead = x.User.Seen,
                CreatedTime = DateTime.Now,
                EntityId = x.CommentId, // Assuming EntityId is the ID of the comment entity
                EntityName = NotificationEntity.React,
                NotificatoinPreview = $"{x.User.UserNames} reacted to your comment.",
                SourceUsername = x.User.UserNames // Assuming UserNames is the name of the user who reacted
            }).ToList() ?? [];

            notificationDto.AddRange(ReactionOnCommentDto);

            return notificationDto;
        }
        #endregion

        #region GetUnreadNotifications
        public List<NotificationsDTO> GetAllUnseenNotification(string userId)
        {
            List<NotificationsDTO> notificationDto = [];
            notificationDto.AddRange(GetUnreadFollowedNotifications(userId));
            notificationDto.AddRange(GetUnreadCommentNotifications(userId));
            notificationDto.AddRange(GetUnreadReactionsNotifications(userId));
            if (notificationDto.Count == 0)
                return [];
            return notificationDto;
        }
        public List<NotificationsDTO> GetUnreadFollowedNotifications(string userId)
        {
            List<NotificationsDTO> notificationDto = [];

            var NotificationBasedOnType = _unitOfWork.CacheRepository<CachedFollowed>()
                                                                                               .GetAsync(userId);
            if (NotificationBasedOnType == null)
                return [];
            var FollowedNotifications = NotificationBasedOnType.Result?.Followers;
            if (FollowedNotifications == null || FollowedNotifications.Count == 0)
                return [];

            var UnseenFollowedNotifications = FollowedNotifications.Where(x => x.Seen == false).ToList();

            notificationDto = [..UnseenFollowedNotifications.Select(x => new NotificationsDTO
                {
                    SourceUserImageUrl = x.ProfileImageUrls,
                    IsRead = x.Seen,
                    CreatedTime = DateTime.Now,
                    EntityId = x.UsersId, // Assuming UsersId is the ID of the user who followed
                    EntityName = NotificationEntity.Follow,
                    NotificatoinPreview = $"{x.UserNames} started following you.",
                    SourceUsername = x.UserNames // Assuming UserNames is the name of the user who followed

                })];
            return notificationDto;
        }
        public List<NotificationsDTO> GetUnreadCommentNotifications(string userId)
        {
            List<NotificationsDTO> notificationDto = [];
            var NotificationBasedOnType = _unitOfWork.CacheRepository<CachedComments>()
                                                                                   .GetAsync(userId);
            if (NotificationBasedOnType == null)
                return [];
            var CommentNotifications = NotificationBasedOnType.Result?.CommnetDetails.Where(x => x.User.Seen == false).ToList();
            if (CommentNotifications == null || CommentNotifications.Count == 0)
                return [];
            notificationDto = [..CommentNotifications.Select(x => new NotificationsDTO
                {
                    SourceUserImageUrl = x.User.ProfileImageUrls,
                    IsRead = x.User.Seen,
                    CreatedTime = DateTime.Now,
                    EntityId = x.CommentId, // Assuming EntityId is the ID of the comment entity
                    EntityName = NotificationEntity.Comment,
                    NotificatoinPreview = $"{x.User.UserNames} commented {x.Content[..Math.Min(20,x.Content.Length)]} on your post.",
                    SourceUsername = x.User.UserNames // Assuming UserNames is the name of the user who commented
                })];
            return notificationDto;

        }
        public List<NotificationsDTO> GetUnreadReactionsNotifications(string userId)
        {
            List<NotificationsDTO> notificationDto = [];
            var NotificationBasedOnType = _unitOfWork.CacheRepository<CachedReactions>()
                                                                                .GetAsync(userId);
            if (NotificationBasedOnType == null)
                return [];
            var UnseenReactionOnPostsNotifications = NotificationBasedOnType.Result?.ReactionsOnPosts
                                                                                                    .Where(x => x.User.Seen == false).ToList();
            if (UnseenReactionOnPostsNotifications == null || UnseenReactionOnPostsNotifications.Count == 0)
                return [];

            notificationDto = [.. UnseenReactionOnPostsNotifications.Select(x => new NotificationsDTO
                {
                    SourceUserImageUrl = x.User.ProfileImageUrls,
                    IsRead=x.User.Seen,
                    CreatedTime= DateTime.Now,
                    EntityId = x.PostId, // Assuming EntityId is the ID of the reaction entity
                    EntityName = NotificationEntity.React,
                    NotificatoinPreview = $"{x.User.UserNames} reacted to your post.",
                    SourceUsername= x.User.UserNames // Assuming UserNames is the name of the user who reacted
                })];

            var UnseenReactionOnCommentNotifications = NotificationBasedOnType.Result?.ReactionsOnComments
                                                                                                        .Where(x => x.User.Seen == false).ToList();

            var ReactionOnCommentDto = UnseenReactionOnCommentNotifications?.Select(x => new NotificationsDTO
            {
                SourceUserImageUrl = x.User.ProfileImageUrls,
                IsRead = x.User.Seen,
                CreatedTime = DateTime.Now,
                EntityId = x.CommentId, // Assuming EntityId is the ID of the comment entity
                EntityName = NotificationEntity.React,
                NotificatoinPreview = $"{x.User.UserNames} reacted to your comment.",
                SourceUsername = x.User.UserNames // Assuming UserNames is the name of the user who reacted
            }).ToList() ?? [];

            notificationDto.AddRange(ReactionOnCommentDto);

            return notificationDto;

        }
        #endregion

        #region MarkNotificationsAsRead
        public async Task<bool> MarkFollowingNotificationAsRead(string userId, string notificationId)
        {
            var NotificationBasedOnType = _unitOfWork.CacheRepository<CachedFollowed>()
                                                                                .GetAsync(userId).Result;
            if (NotificationBasedOnType == null)
                return false;

            var FollowedNotifications = NotificationBasedOnType?.Followers;

            if (FollowedNotifications == null || FollowedNotifications.Count == 0)
                return false;


            var notification = FollowedNotifications.FirstOrDefault(x => x.Id == notificationId);
            notification!.Seen = true; // Mark the notification as read
            await _unitOfWork.CacheRepository<CachedFollowed>().UpdateAsync(NotificationBasedOnType!, notificationId);

            return true;
        }
        public async Task<bool> MarkCommentNotificationAsRead(string userId, string notificationId)
        {
            var NotificationBasedOnType = _unitOfWork.CacheRepository<CachedComments>()
                                                                                   .GetAsync(userId).Result;
            if (NotificationBasedOnType == null)
                return false;
            var CommentNotifications = NotificationBasedOnType?.CommnetDetails;
            if (CommentNotifications == null || CommentNotifications.Count == 0)
                return false;
            var notification = CommentNotifications.FirstOrDefault(x => x.User.Id == notificationId);
            notification!.User.Seen = true; // Mark the notification as read
            await _unitOfWork.CacheRepository<CachedComments>().UpdateAsync(NotificationBasedOnType!, notificationId);
            return true;

        }

        public async Task<bool> MarkReactionNotificationAsRead(string userId, string notificationId)
        {
            var NotificationBasedOnType = _unitOfWork.CacheRepository<CachedReactions>()
                                                                                  .GetAsync(userId).Result;
            if (NotificationBasedOnType == null)
                return false;

            var ReactionOnPostsNotifications = NotificationBasedOnType?.ReactionsOnPosts;
            if (ReactionOnPostsNotifications == null || ReactionOnPostsNotifications.Count == 0)
                return await MarkCommentNotificationAsRead(userId, notificationId); // Fallback to comment notification if no post reactions found


            var notification = ReactionOnPostsNotifications.FirstOrDefault(x => x.User.Id == notificationId);
            notification!.User.Seen = true; // Mark the notification as read
            await _unitOfWork.CacheRepository<CachedReactions>().UpdateAsync(NotificationBasedOnType!, notificationId);
            return true;
        }

        private async Task<bool> MarkCommentReactionNotificationAsRead(string userId, string notificationId)
        {
            var NotificationBasedOnType = _unitOfWork.CacheRepository<CachedReactions>()
                                                                                  .GetAsync(userId).Result;
            if (NotificationBasedOnType == null)
                return false;

            var ReactionOnCommentsNotifications = NotificationBasedOnType?.ReactionsOnComments;
            if (ReactionOnCommentsNotifications == null || ReactionOnCommentsNotifications.Count == 0)
                return false;

            var notification = ReactionOnCommentsNotifications.FirstOrDefault(x => x.User.Id == notificationId);
            notification!.User.Seen = true; // Mark the notification as read
            await _unitOfWork.CacheRepository<CachedReactions>().UpdateAsync(NotificationBasedOnType!, notificationId);
            return true;
        }
        #endregion

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
