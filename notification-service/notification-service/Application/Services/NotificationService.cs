using Application.DTO;
using Application.Interfaces;
using Domain.CacheEntities;
using Domain.CacheEntities.Comments;
using Domain.CacheEntities.Reactions;
using Domain.CoreEntities;
using Domain.Enums;
using Domain.Interfaces;
using System.ComponentModel.Design;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Application.Services
{

    public class NotificationService(IUnitOfWork unitOfWork1) : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork1;

        // add source user Id
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
                    SourceUsername= x.UserNames, // Assuming UserNames is the name of the user who followed

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
            return new List<NotificationsDTO>();
        }


        // mark notifications as read for follow , comment , reaction post , reaction commen -------------------
        public async Task<bool> MarkNotificationsFollowAsRead(string userId , string userFollowedId )
        {
            // cache repo Found , null , not Found 
            var usercached = await  _unitOfWork.CacheRepository<CachedFollowed>().GetSingleAsync(x => x.UserId ==  userId);
            if (usercached == null) 
            {
                throw new ArgumentException("UserId not Found  in Cash");
            }
            var userFollowed = usercached.Followers.FirstOrDefault(i => i.UsersId == userFollowedId);
            if (userFollowed == null)
            {
                throw new ArgumentException("user userFollowedId not Found in Cash");

            }
            userFollowed.Seen = true;
            await _unitOfWork.CacheRepository<CachedFollowed>().UpdateAsync(usercached, usercached.UserId);

            var userCore = await _unitOfWork.CoreRepository<Follows>()
                .GetSingleIncludingAsync(
                    f => f.Id == userId,
                    i => i.FollowsNotifReadByAuthor);

            if (userCore ==  null)
            {
                throw new ArgumentException("UserId not Found in Core");
              
            }

            if (!userCore.FollowsNotifReadByAuthor.Any(r => r == userFollowedId))
            {
                userCore.FollowsNotifReadByAuthor.Add(userFollowedId);
                await _unitOfWork.CoreRepository<Follows>().UpdateAsync(userCore);
                await _unitOfWork.SaveChangesAsync();
            }
            return true;
                 

        }
        public async Task<bool> MarkNotificationsCommentAsRead(string userId,string CommentId)
        {

            // cache repo Found , null , not Found 
            var usercached = await _unitOfWork.CacheRepository<CachedComments>().GetSingleAsync(x => x.UserId == userId);
    
            if (usercached == null) 
            {
                throw new ArgumentException("UserId not Found  in Cash");
            }
            var userComment = usercached.CommnetDetails.FirstOrDefault(i => i.CommentId ==  CommentId );
            if (userComment == null)
            {
                throw new ArgumentException("user CommentId not Found in Cash");

            }
            if (!userComment.IsRead)
            {
                userComment.IsRead = true;
                await _unitOfWork.CacheRepository<CachedComments>().UpdateAsync(usercached, usercached.UserId);
            }
        

            var userCore = await _unitOfWork.CoreRepository<Comment>()
                .GetSingleIncludingAsync(
                    f => f.Id == userId,
                    f => f.CommentNotifReadByAuthor);

            if (userCore == null)
            {
                throw new ArgumentException("UserId not Found in Core");

            }

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
            var usercached = await _unitOfWork.CacheRepository<CachedReactions>().GetSingleAsync(x => x.AuthorId== userId);

            if (usercached == null)
            {
                throw new ArgumentException("UserId not Found  in Cash");
            }
            var userReactPost = usercached.ReactionsOnPosts.FirstOrDefault(i => i.ReactionId == reactionId);
            if (userReactPost == null)
            {
                throw new ArgumentException("user ReactionPost not Found in Cash");

            }
            if (!userReactPost.IsRead)
            {
                userReactPost.IsRead = true;
                await _unitOfWork.CacheRepository<CachedReactions>().UpdateAsync(usercached,usercached.AuthorId);
            }


            var userCore = await _unitOfWork.CoreRepository<Reaction>()
                .GetSingleIncludingAsync(
                    f => f.Id == userId,
                    f => f.PostReactionsNotifReadByAuthor);

            if (userCore == null)
            {
                throw new ArgumentException("UserId not Found in Core");

            }

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
            var usercached = await _unitOfWork.CacheRepository<CachedReactions>().GetSingleAsync(x => x.AuthorId == userId);

            if (usercached == null)
            {
                throw new ArgumentException("UserId not Found  in Cash");
            }
            var userReactComment = usercached.ReactionsOnComments.FirstOrDefault(i => i.ReactionId == reactionId);
            if (userReactComment == null)
            {
                throw new ArgumentException("userReactComment not Found in Cash");

            }
            if (!userReactComment.IsRead)
            {
                userReactComment.IsRead = true;
                await _unitOfWork.CacheRepository<CachedReactions>().UpdateAsync(usercached, usercached.AuthorId);
            }


            var userCore = await _unitOfWork.CoreRepository<Reaction>()
                .GetSingleIncludingAsync(
                    f => f.Id == userId,
                    f => f.CommentReactionsNotifReadByAuthor);

            if (userCore == null)
            {
                throw new ArgumentException("UserId not Found in Core");

            }

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
                .GetSingleAsync(x => x.AuthorId == userId);
            if (userCached == null)
            {
                throw new ArgumentException($"User {userId} not found in cache");
            }
            if ((userCached.ReactionsOnPosts == null || !userCached.ReactionsOnPosts.Any())  && (userCached.ReactionsOnComments == null || !userCached.ReactionsOnComments.Any()) )
            {
                return true; // No reactions to mark as read
            }
            bool anyUnread = false;
            foreach (var reaction in userCached.ReactionsOnPosts)
            {
                if (!reaction.IsRead)
                {
                    reaction.IsRead = true;
                    anyUnread = true;
                }
            }
        
            foreach (var reaction in userCached.ReactionsOnComments)
            {
                if (!reaction.IsRead)
                {
                    reaction.IsRead = true;
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
                .GetSingleIncludingAsync(r => r.Id == userId, includes.ToArray());
            if (userCore == null)
            {
                throw new ArgumentException($"User {userId} not found in core database");
            }
            if (userCore.PostReactionsNotifReadByAuthor == null)
            {
                userCore.PostReactionsNotifReadByAuthor = [];
            }
            if (userCore.CommentReactionsNotifReadByAuthor == null)
            {
                userCore.CommentReactionsNotifReadByAuthor = [];
            }
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
                .GetSingleAsync(x => x.UserId == userId);

            if (userCached == null)
            {
                throw new ArgumentException($"User {userId} not found in cache");
            }

            if (userCached.Followers == null || !userCached.Followers.Any())
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
                .GetSingleIncludingAsync(f => f.Id == userId, includes.ToArray());

            if (userCore == null)
            {
                throw new ArgumentException($"User {userId} not found in core database");
            }

            if (userCore.FollowersId == null || !userCore.FollowersId.Any())
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
                .GetSingleAsync(x => x.UserId == userId);

            if (userCached == null)
            {
                throw new ArgumentException($"User {userId} not found in cache");
            }

            if (userCached.CommnetDetails == null || !userCached.CommnetDetails.Any())
            {
                return true; // No comment notifications to mark as read
            }

            bool anyUnread = false;
            foreach (var comment in userCached.CommnetDetails)
            {
                if (!comment.IsRead)
                {
                    comment.IsRead = true;
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
                .GetSingleIncludingAsync(c => c.Id == userId, includes.ToArray());

            if (userCore == null)
            {
                throw new ArgumentException($"User {userId} not found in core database");
            }

            if (userCore.UserID_CommentId == null || !userCore.UserID_CommentId.Any())
            {
                return true; // No comments to process
            }

            bool needsUpdate = false;

            if (userCore.UserID_CommentId?.Any() == true)
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

                if (newReadComments.Any())
                {
                    // Use AddRange instead of individual Add calls
                    userCore.CommentNotifReadByAuthor.AddRange(newReadComments);

                    // Update the entity
                    await _unitOfWork.CoreRepository<Comment>().UpdateAsync(userCore);
                    await _unitOfWork.SaveChangesAsync();
                }
            }


            return true;
        }

        public async  Task<bool> MarkNotificationAsUnread(string userId, string notificationId)
        {
            // Logic to mark a specific notification as unread for the user
            return true;
        }
        public Task<List<NotificationEntity>> GetNotificationTypes()
        {
            var types = Enum.GetValues(typeof(NotificationEntity))
                            .Cast<NotificationEntity>()
                            .ToList();

            return Task.FromResult(types);
        }




    }
}
