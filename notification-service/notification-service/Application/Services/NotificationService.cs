using Application.DTO;
using Application.Interfaces;
using Domain.CacheEntities;
using Domain.CacheEntities.Comments;
using Domain.CacheEntities.Reactions;
using Domain.Enums;
using Domain.Interfaces;

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

                notificationDto.AddRange([.. CommentNotifications.Select(x => new NotificationsDTO
                {
                    SourceUserImageUrl = x.User.ProfileImageUrls,
                    IsRead=x.User.Seen,
                    CreatedTime= DateTime.Now,
                    EntityId = x.CommentId, // Assuming EntityId is the ID of the comment entity
                    EntityName = NotificationEntity.Comment,
                    NotificatoinPreview = $"{x.User.UserNames} commented on your post.",
                    SourceUsername= x.User.UserNames // Assuming UserNames is the name of the user who commented
                })]);

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

        public List<NotificationsDTO> UnreadNotifications(string userId, NotificationEntity notificationType)
        {
            List<NotificationsDTO> notificationDto = [];
            if (string.IsNullOrEmpty(userId))
                return [];

            if (notificationType == NotificationEntity.All)
            {
                //////////////////////////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////////////// Following ///////////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////////////

                var AllachedFollowed = _unitOfWork.CacheRepository<CachedFollowed>()
                                                                                   .GetAsync(userId).Result;
                if (AllachedFollowed == null)
                    return [];
                var cachedFollowed = AllachedFollowed.Followers.Where(x => x.Seen == false).ToList();

                if (cachedFollowed == null || cachedFollowed.Count == 0)
                    return [];

                notificationDto = [.. cachedFollowed.Select(x => new NotificationsDTO
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
                                                                                   .GetAsync(userId).Result?.CommnetDetails;
                if (CachedComments == null || CachedComments.Count == 0)
                    return [];

                var unseenCommentNotifications = CachedComments.Where(x => x.User.Seen == false).ToList();

                notificationDto.AddRange([.. unseenCommentNotifications.Select(x => new NotificationsDTO
                {
                    SourceUserImageUrl = x.User.ProfileImageUrls,
                    IsRead=x.User.Seen,
                    CreatedTime= DateTime.Now,
                    EntityId = x.CommentId, // Assuming EntityId is the ID of the comment entity
                    EntityName = NotificationEntity.Comment,
                    NotificatoinPreview = $"{x.User.UserNames} commented {x.Content[..Math.Min(20, x.Content.Length)]} on your post.",
                    SourceUsername= x.User.UserNames // Assuming UserNames is the name of the user who commented
                })]);

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

                var unseenReactionOnPostsNotifications = ReactionOnPostsNotifications.Where(x => x.User.Seen == false).ToList();

                notificationDto.AddRange([.. unseenReactionOnPostsNotifications.Select(x => new NotificationsDTO
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

                var unseenReactionOnCommentNotifications = ReactionOnCommentNotifications.Where(x => x.User.Seen == false).ToList();

                notificationDto.AddRange(unseenReactionOnCommentNotifications?.Select(x => new NotificationsDTO
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

            else if (notificationType == NotificationEntity.Comment)
            {
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

            else if (notificationType == NotificationEntity.React)
            {
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

            throw new ArgumentException("Invalid notification type provided.");
        }

        public bool MarkNotificationAsRead(string userId, NotificationEntity notificationEntity, string notificationId)
        {
            if (notificationEntity == NotificationEntity.Follow)
            {
                var NotificationBasedOnType = _unitOfWork.CacheRepository<CachedFollowed>()
                                                                                    .GetAsync(userId).Result?.Followers;

                if (NotificationBasedOnType == null || NotificationBasedOnType.Count == 0)
                    return false;


                //var notification = NotificationBasedOnType.FirstOrDefault(x => x.UsersId == notificationId);

            }
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
