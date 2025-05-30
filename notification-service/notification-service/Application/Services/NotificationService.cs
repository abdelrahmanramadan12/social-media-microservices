using Application.DTO;
using Application.Interfaces;
using Domain.CacheEntities;
using Domain.CacheEntities.Comments;
using Domain.CacheEntities.Reactions;
using Domain.CoreEntities;
using Domain.Enums;
using Domain.Interfaces;
using System.Linq.Expressions;

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
                    EntityId = x.ReactionId, // Assuming EntityId is the ID of the reaction entity
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
                EntityId = x.ReactionId, // Assuming EntityId is the ID of the comment entity
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
        //public async Task<bool> MarkFollowingNotificationAsRead(string userId, string notificationId)
        //{
        //    var NotificationBasedOnType = _unitOfWork.CacheRepository<CachedFollowed>()
        //                                                                        .GetAsync(userId).Result;
        //    if (NotificationBasedOnType == null)
        //        return false;

        //    var FollowedNotifications = NotificationBasedOnType?.Followers;

        //    if (FollowedNotifications == null || FollowedNotifications.Count == 0)
        //        return false;


        //    var notification = FollowedNotifications.FirstOrDefault(x => x.Id == notificationId);
        //    notification!.Seen = true; // Mark the notification as read
        //    await _unitOfWork.CacheRepository<CachedFollowed>().UpdateAsync(NotificationBasedOnType!, notificationId);

        //    return true;
        //}
        //public async Task<bool> MarkCommentNotificationAsRead(string userId, string notificationId)
        //{
        //    var NotificationBasedOnType = _unitOfWork.CacheRepository<CachedComments>()
        //                                                                           .GetAsync(userId).Result;
        //    if (NotificationBasedOnType == null)
        //        return false;
        //    var CommentNotifications = NotificationBasedOnType?.CommnetDetails;
        //    if (CommentNotifications == null || CommentNotifications.Count == 0)
        //        return false;
        //    var notification = CommentNotifications.FirstOrDefault(x => x.User.Id == notificationId);
        //    notification!.User.Seen = true; // Mark the notification as read
        //    await _unitOfWork.CacheRepository<CachedComments>().UpdateAsync(NotificationBasedOnType!, notificationId);
        //    return true;

        //}

        //public async Task<bool> MarkReactionNotificationAsRead(string userId, string notificationId)
        //{
        //    var NotificationBasedOnType = _unitOfWork.CacheRepository<CachedReactions>()
        //                                                                          .GetAsync(userId).Result;
        //    if (NotificationBasedOnType == null)
        //        return false;

        //    var ReactionOnPostsNotifications = NotificationBasedOnType?.ReactionsOnPosts;
        //    if (ReactionOnPostsNotifications == null || ReactionOnPostsNotifications.Count == 0)
        //        return await MarkCommentNotificationAsRead(userId, notificationId); // Fallback to comment notification if no post reactions found


        //    var notification = ReactionOnPostsNotifications.FirstOrDefault(x => x.User.Id == notificationId);
        //    notification!.User.Seen = true; // Mark the notification as read
        //    await _unitOfWork.CacheRepository<CachedReactions>().UpdateAsync(NotificationBasedOnType!, notificationId);
        //    return true;
        //}


        // mark notifications as read for follow , comment , reaction post , reaction commen -------------------

        public async Task<bool> MarkNotificationsFollowAsRead(string userId, string userFollowedId)
        {
            // cache repo Found , null , not Found 
            var usercached = await _unitOfWork.CacheRepository<CachedFollowed>().GetSingleAsync(x => x.UserId == userId)
                                                             ?? throw new ArgumentException("UserId not Found  in Cash");
            var userFollowed = usercached.Followers.FirstOrDefault(i => i.UsersId == userFollowedId)
                                                             ?? throw new ArgumentException("user userFollowedId not Found in Cash");
            userFollowed.Seen = true;
            await _unitOfWork.CacheRepository<CachedFollowed>().UpdateAsync(usercached, usercached.UserId);

            var userCore = await _unitOfWork.CoreRepository<Follows>()
                .GetSingleIncludingAsync(
                    f => f.Id == userId,
                    i => i.FollowsNotifReadByAuthor) ?? throw new ArgumentException("UserId not Found in Core");

            if (!userCore.FollowsNotifReadByAuthor.Any(r => r == userFollowedId))
            {
                userCore.FollowsNotifReadByAuthor.Add(userFollowedId);
                await _unitOfWork.CoreRepository<Follows>().UpdateAsync(userCore);
                await _unitOfWork.SaveChangesAsync();
            }
            return true;


        }
        #endregion

        public async Task<bool> MarkNotificationsCommentAsRead(string userId, string CommentId)
        {

            // cache repo Found , null , not Found 
            var usercached = await _unitOfWork.CacheRepository<CachedComments>().GetSingleAsync(x => x.UserId == userId);

            if (usercached == null)
            {
                throw new ArgumentException("UserId not Found  in Cash");
            }
            var userComment = usercached.CommnetDetails.FirstOrDefault(i => i.CommentId == CommentId)
                                                                                ?? throw new ArgumentException("user CommentId not Found in Cash");
            if (!userComment.User.Seen)
            {
                userComment.User.Seen = true;
                await _unitOfWork.CacheRepository<CachedComments>().UpdateAsync(usercached, usercached.UserId);
            }


            var userCore = await _unitOfWork.CoreRepository<Comment>()
                .GetSingleIncludingAsync(
                    f => f.Id == userId,
                    f => f.CommentNotifReadByAuthor) ?? throw new ArgumentException("UserId not Found in Core");

            if (!userCore.CommentNotifReadByAuthor.Any(r => r == CommentId))
            {
                userCore.CommentNotifReadByAuthor.Add(CommentId);
                await _unitOfWork.CoreRepository<Comment>().UpdateAsync(userCore);
                await _unitOfWork.SaveChangesAsync();
            }
            return true;

        }

        public async Task<bool> MarkNotificationsReactionPostAsRead(string userId, string reactionId)
        {

            // cache repo Found , null , not Found 
            var usercached = await _unitOfWork.CacheRepository<CachedReactions>().GetSingleAsync(x => x.AuthorId == userId);

            if (usercached == null)
            {
                throw new ArgumentException("UserId not Found  in Cash");
            }
            var userReactPost = usercached.ReactionsOnPosts.FirstOrDefault(i => i.ReactionId == reactionId)
                                                                                ?? throw new ArgumentException("user ReactionPost not Found in Cash");
            if (!userReactPost.User.Seen)
            {
                userReactPost.User.Seen = true;
                await _unitOfWork.CacheRepository<CachedReactions>().UpdateAsync(usercached, usercached.AuthorId);
            }


            var userCore = await _unitOfWork.CoreRepository<Reaction>()
                .GetSingleIncludingAsync(
                    f => f.Id == userId,
                    f => f.PostReactionsNotifReadByAuthor) ?? throw new ArgumentException("UserId not Found in Core");

            if (!userCore.PostReactionsNotifReadByAuthor.Any(r => r == reactionId))
            {
                userCore.PostReactionsNotifReadByAuthor.Add(reactionId);
                await _unitOfWork.CoreRepository<Reaction>().UpdateAsync(userCore);
                await _unitOfWork.SaveChangesAsync();
            }
            return true;

        }


        public async Task<bool> MarkNotificationsReactionCommentAsRead(string userId, string reactionId)
        {

            // cache repo Found , null , not Found 
            var usercached = await _unitOfWork.CacheRepository<CachedReactions>().GetSingleAsync(x => x.AuthorId == userId)
                                                                                    ?? throw new ArgumentException("UserId not Found  in Cash");

            var userReactComment = usercached.ReactionsOnComments.FirstOrDefault(i => i.ReactionId == reactionId)
                                                                                ?? throw new ArgumentException("userReactComment not Found in Cash");

            if (!userReactComment.User.Seen)
            {
                userReactComment.User.Seen = true;
                await _unitOfWork.CacheRepository<CachedReactions>().UpdateAsync(usercached, usercached.AuthorId);
            }


            var userCore = await _unitOfWork.CoreRepository<Reaction>()
                .GetSingleIncludingAsync(
                    f => f.Id == userId,
                    f => f.CommentReactionsNotifReadByAuthor) ?? throw new ArgumentException("UserId not Found in Core");

            if (!userCore.CommentReactionsNotifReadByAuthor.Any(r => r == reactionId))
            {
                userCore.CommentReactionsNotifReadByAuthor.Add(reactionId);
                await _unitOfWork.CoreRepository<Reaction>().UpdateAsync(userCore);
                await _unitOfWork.SaveChangesAsync();
            }
            return true;

        }

        // mark all not reactions 
        public async Task<bool> MarkAllNotificationsAsRead(string userId)
        {
            try
            {
                await Task.WhenAll(
                    MarkAllNotificationsCommentAsRead(userId),
                    MarkAllNotificationsFollowAsRead(userId),
                    MarkAllNotificationsReactionsAsRead(userId)
                );
                return true;
            }
            catch (Exception ex)
            {
                // Log error here
                throw new ApplicationException("Failed to mark all notifications as read", ex);
            }
        }
        public async Task<bool> MarkAllNotificationsReactionsAsRead(string userId)
        {
            // Cache update
            var userCached = await _unitOfWork.CacheRepository<CachedReactions>()
                .GetSingleAsync(x => x.AuthorId == userId) ?? throw new ArgumentException($"User {userId} not found in cache");

            if ((userCached.ReactionsOnPosts == null || userCached.ReactionsOnPosts.Count == 0) &&
                (userCached.ReactionsOnComments == null || userCached.ReactionsOnComments.Count == 0))
            {
                return true; // No reactions to mark as read
            }
            bool anyUnread = false;
            foreach (var reaction in userCached.ReactionsOnPosts!)
            {
                if (!reaction.User.Seen)
                {
                    reaction.User.Seen = true;
                    anyUnread = true;
                }
            }

            foreach (var reaction in userCached.ReactionsOnComments)
            {
                if (!reaction.User.Seen)
                {
                    reaction.User.Seen = true;
                    anyUnread = true;
                }
            }
            if (anyUnread)
            {
                await _unitOfWork.CacheRepository<CachedReactions>()
                    .UpdateAsync(userCached, userCached.AuthorId);
            }
            // Core DB update
            var includes = new List<Expression<Func<Reaction, object>>>
            {
                r => r.PostReactionsNotifReadByAuthor,
                r => r.CommentReactionsNotifReadByAuthor
            };
            var userCore = await _unitOfWork.CoreRepository<Reaction>()
                .GetSingleIncludingAsync(r => r.Id == userId, [.. includes])
                                            ?? throw new ArgumentException($"User {userId} not found in core database");

            userCore.PostReactionsNotifReadByAuthor ??= [];
            userCore.CommentReactionsNotifReadByAuthor ??= [];

            bool needsUpdate = false;
            foreach (var reaction in userCached.ReactionsOnPosts)
            {
                if (!userCore.PostReactionsNotifReadByAuthor.Any(r => r == reaction.ReactionId))
                {
                    userCore.PostReactionsNotifReadByAuthor.Add(reaction.ReactionId);
                    needsUpdate = true;
                }
            }
            foreach (var reaction in userCached.ReactionsOnComments)
            {
                if (!userCore.CommentReactionsNotifReadByAuthor.Any(r => r == reaction.ReactionId))
                {
                    userCore.CommentReactionsNotifReadByAuthor.Add(reaction.ReactionId);
                    needsUpdate = true;
                }
            }
            if (needsUpdate)
            {
                await _unitOfWork.CoreRepository<Reaction>().UpdateAsync(userCore);
                await _unitOfWork.SaveChangesAsync();
            }


            return true;
        }
        public async Task<bool> MarkAllNotificationsFollowAsRead(string userId)
        {
            // Cache update
            var userCached = await _unitOfWork.CacheRepository<CachedFollowed>()
                .GetSingleAsync(x => x.UserId == userId) ?? throw new ArgumentException($"User {userId} not found in cache");

            if (userCached.Followers == null || userCached.Followers.Count == 0)
            {
                return true; // No notifications to mark as read
            }

            bool anyUnread = false;
            foreach (var notification in userCached.Followers)
            {
                if (!notification.Seen)
                {
                    notification.Seen = true;
                    anyUnread = true;
                }
            }

            if (anyUnread)
            {
                await _unitOfWork.CacheRepository<CachedFollowed>()
                    .UpdateAsync(userCached, userCached.UserId);
            }

            // Core DB update
            var includes = new List<Expression<Func<Follows, object>>>
            {
                f => f.FollowsNotifReadByAuthor,
                f => f.FollowersId
            };

            var userCore = await _unitOfWork.CoreRepository<Follows>()
                .GetSingleIncludingAsync(f => f.Id == userId, [.. includes])
                                            ?? throw new ArgumentException($"User {userId} not found in core database");
            if (userCore.FollowersId == null || userCore.FollowersId.Count == 0)
            {
                return true; // No followers to process
            }

            bool needsUpdate = false;
            foreach (var userFollowedId in userCore.FollowersId)
            {
                if (!userCore.FollowsNotifReadByAuthor.Any(r => r == userFollowedId))
                {
                    userCore.FollowsNotifReadByAuthor.Add(userFollowedId);
                    needsUpdate = true;
                }
            }

            if (needsUpdate)
            {
                await _unitOfWork.CoreRepository<Follows>().UpdateAsync(userCore);
                await _unitOfWork.SaveChangesAsync();
            }

            return true;
        }


        public async Task<bool> MarkAllNotificationsCommentAsRead(string userId)
        {
            // Cache update
            var userCached = await _unitOfWork.CacheRepository<CachedComments>()
                .GetSingleAsync(x => x.UserId == userId) ?? throw new ArgumentException($"User {userId} not found in cache");

            if (userCached.CommnetDetails == null || userCached.CommnetDetails.Count == 0)
            {
                return true; // No comment notifications to mark as read
            }

            bool anyUnread = false;
            foreach (var comment in userCached.CommnetDetails)
            {
                if (!comment.User.Seen)
                {
                    comment.User.Seen = true;
                    anyUnread = true;
                }
            }

            if (anyUnread)
            {
                await _unitOfWork.CacheRepository<CachedComments>()
                    .UpdateAsync(userCached, userCached.UserId);
            }

            // Core DB update
            var includes = new List<Expression<Func<Comment, object>>>
            {
                c => c.CommentNotifReadByAuthor,
                c => c.UserID_CommentId // or whatever property contains all comment IDs
            };

            var userCore = await _unitOfWork.CoreRepository<Comment>()
                .GetSingleIncludingAsync(c => c.Id == userId, [.. includes])
                                        ?? throw new ArgumentException($"User {userId} not found in core database");

            if (userCore.UserID_CommentId == null || userCore.UserID_CommentId.Count == 0)
            {
                return true; // No comments to process
            }
            if (userCore.UserID_CommentId?.Count == 0)
            {
                // Get all unique comment IDs from dictionaries
                var allCommentIds = userCore.UserID_CommentId
                    .SelectMany(d => d.Values)
                    .Distinct()
                    .ToList();

                // Find only new unread comments
                var newReadComments = allCommentIds
                    .Except(userCore.CommentNotifReadByAuthor ?? Enumerable.Empty<string>())
                    .ToList();

                if (newReadComments.Count != 0)
                {
                    // Use AddRange instead of individual Add calls
                    userCore.CommentNotifReadByAuthor?.AddRange(newReadComments);

                    // Update the entity
                    await _unitOfWork.CoreRepository<Comment>().UpdateAsync(userCore);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            return true;
        }

        public async Task<List<NotificationEntity>> GetNotificationTypes()
        {
            var types = Enum.GetValues(typeof(NotificationEntity))
                            .Cast<NotificationEntity>()
                            .ToList();

            return await Task.FromResult(types);
        }
    }
}
