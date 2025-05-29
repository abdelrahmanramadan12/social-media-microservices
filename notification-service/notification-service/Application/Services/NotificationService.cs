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

        public List<NotificationsDTO> GetNotificationsByType(string userId, NotificationEntity notificationType)
        {
            List<NotificationsDTO> notificationDto = [];
            if (string.IsNullOrEmpty(userId))
                return [];

            if (notificationType == NotificationEntity.All)
            {

                //////////////////////////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////////////// Following ///////////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////////////

                var cachedFollowed = _unitOfWork.CacheRepository<CachedFollowed>()
                                                                                   .GetAsync(userId);
                if (cachedFollowed == null)
                    return [];
                var FollowedNotifications = cachedFollowed.Result?.Followers;
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

                //////////////////////////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////////////// Comments ////////////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////////////

                var CachedComments = _unitOfWork.CacheRepository<CachedComments>()
                                                                                   .GetAsync(userId);
                if (cachedFollowed == null)
                    return [];
                var CommentNotifications = CachedComments.Result?.CommnetDetails;
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

                //////////////////////////////////////////////////////////////////////////////////////////////////
                /////////////////////////////////////// Reactions on posts ///////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////////////

                var CachedReactions = _unitOfWork.CacheRepository<CachedReactions>()
                                                                                  .GetAsync(userId);
                if (CachedReactions == null)
                    return [];
                var ReactionOnPostsNotifications = CachedReactions.Result?.ReactionsOnPosts;
                if (ReactionOnPostsNotifications == null || ReactionOnPostsNotifications.Count == 0)
                    return [];


                notificationDto.AddRange([.. ReactionOnPostsNotifications.Select(x => new NotificationsDTO
                {
                    SourceUserImageUrl = x.User.ProfileImageUrls,
                    IsRead=x.User.Seen,
                    CreatedTime= DateTime.Now,
                    EntityId = x.PostId, // Assuming EntityId is the ID of the reaction entity
                    EntityName = NotificationEntity.React,
                    NotificatoinPreview = $"{x.User.UserNames} reacted to your post.",
                    SourceUsername= x.User.UserNames // Assuming UserNames is the name of the user who reacted
                })]);

                //////////////////////////////////////////////////////////////////////////////////////////////////
                /////////////////////////////////////// Reactions on comment /////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////////////

                var ReactionOnCommentNotifications = CachedReactions.Result?.ReactionsOnComments;

                if (ReactionOnCommentNotifications == null || ReactionOnCommentNotifications.Count == 0)
                    return [];

                notificationDto.AddRange(ReactionOnCommentNotifications?.Select(x => new NotificationsDTO
                {
                    SourceUserImageUrl = x.User.ProfileImageUrls,
                    IsRead = x.User.Seen,
                    CreatedTime = DateTime.Now,
                    EntityId = x.CommentId, // Assuming EntityId is the ID of the comment entity
                    EntityName = NotificationEntity.React,
                    NotificatoinPreview = $"{x.User.UserNames} reacted to your comment.",
                    SourceUsername = x.User.UserNames // Assuming UserNames is the name of the user who reacted
                }).ToList() ?? []);
            }

            else if (notificationType == NotificationEntity.Follow)
            {
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

            else if (notificationType == NotificationEntity.Comment)
            {
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

            else if (notificationType == NotificationEntity.React)
            {
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

            throw new ArgumentException("Invalid notification type provided.");
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
