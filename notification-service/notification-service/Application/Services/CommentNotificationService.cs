using Application.DTO;
using Application.Hubs;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Application.Services;
using Domain.CacheEntities;
using Domain.CacheEntities.Comments;
using Domain.CoreEntities;
using Domain.Events;
using Microsoft.AspNetCore.SignalR;

namespace Application.Services
{

    public class CommentNotificationService(IUnitOfWork unitOfWork, IRealtimeNotifier _realtimeNotifier , IProfileServiceClient profileServiceClient)
        : ICommentNotificationService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;
        private readonly IRealtimeNotifier realtimeNotifier = _realtimeNotifier;
        private readonly IProfileServiceClient profileServiceClient = profileServiceClient;

        public async Task UpdatCommentListNotification(CommentEvent commentEvent)
        {
            // --- CORE DB: Load or create author comment notification object ---
            var authorNotification = await unitOfWork.CoreRepository<Comments>()
                .GetSingleAsync(x => x.PostAuthorId == commentEvent.PostAuthorId);

            if (authorNotification == null)
            {
                authorNotification = new Comments
                {
                    PostAuthorId = commentEvent.PostAuthorId,
                    UserID_CommentIds = new Dictionary<string, List<string>>()
                };
            }

            // Ensure comment list for commentor exists
            if (!authorNotification.UserID_CommentIds.TryGetValue(commentEvent.CommentAuthorId, out var commentList))
            {
                commentList = new List<string>();
                authorNotification.UserID_CommentIds[commentEvent.CommentAuthorId] = commentList;
            }

            commentList.Add(commentEvent.CommentId);

            // --- Ensure UserSkeleton is cached ---
            var userSkeleton = await unitOfWork.CacheRepository<UserSkeleton>()
                .GetSingleAsync(u => u.UserId == commentEvent.CommentAuthorId);

            if (userSkeleton == null)
            {
                var profile = await profileServiceClient.GetProfileAsync(commentEvent.CommentAuthorId);
                if (profile?.Data == null)
                    return; // can't continue without profile

                userSkeleton = new UserSkeleton
                {
                    UserId = profile.Data.UserId,
                    UserNames = profile.Data.UserNames,
                    ProfileImageUrls = profile.Data.ProfileImageUrl,
                    CreatedAt = DateTime.UtcNow,
                    Seen = false
                };

                await unitOfWork.CacheRepository<UserSkeleton>().AddAsync(userSkeleton);
            }

            // --- Update or create CachedCommentsNotification for each user in map ---
            foreach (var userId in authorNotification.UserID_CommentIds.Keys)
            {
                var cacheUser = await unitOfWork.CacheRepository<CachedCommentsNotification>()
                    .GetSingleByIdAsync(userId);

                if (cacheUser == null)
                {
                    cacheUser = new CachedCommentsNotification
                    {
                        UserId = userId,
                        CommnetDetails = new List<CommnetNotificationDetails>()
                    };

                    await unitOfWork.CacheRepository<CachedCommentsNotification>().AddAsync(cacheUser);
                }

                cacheUser.CommnetDetails ??= new List<CommnetNotificationDetails>();

                cacheUser.CommnetDetails.Add(new CommnetNotificationDetails
                {
                    CommentId = commentEvent.CommentId,
                    PostId = commentEvent.PostId,
                    User = userSkeleton
                });

                await unitOfWork.CacheRepository<CachedCommentsNotification>().UpdateAsync(cacheUser);

                // Send notification
                await realtimeNotifier.SendMessageAsync(userId, new NotificationsDTO
                {
                    EntityName = Domain.Enums.NotificationEntity.Comment,
                    SourceUsername = userSkeleton.UserNames,
                    SourceUserImageUrl = userSkeleton.ProfileImageUrls,
                    CreatedTime = DateTime.UtcNow,
                });
            }

            // --- Save all updates ---
            await unitOfWork.CoreRepository<Comments>().UpdateAsync(authorNotification);
            await unitOfWork.SaveChangesAsync();
        }


        public async Task RemoveCommentListNotification(CommentEvent commentEvent)
        {
            // Get the core notification for the post author
            var authorNotification = await unitOfWork.CoreRepository<Comments>()
                .GetSingleIncludingAsync(n => n.PostAuthorId == commentEvent.PostAuthorId);

            if (authorNotification == null)
                return;

            // Safely remove the comment ID from the core dictionary
            if (authorNotification.UserID_CommentIds.TryGetValue(commentEvent.CommentAuthorId, out var commentList))
            {
                commentList.Remove(commentEvent.CommentId);

                // Clean up the entry if there are no more comments by this user
                if (commentList.Count == 0)
                    authorNotification.UserID_CommentIds.Remove(commentEvent.CommentAuthorId);
            }

            // Update the user's cached comment notification
            var cacheUser = await unitOfWork.CacheRepository<CachedCommentsNotification>()
                .GetSingleByIdAsync(commentEvent.CommentAuthorId);

            if (cacheUser != null)
            {
                cacheUser.CommnetDetails?.RemoveAll(cd =>
                    cd.CommentId == commentEvent.CommentId &&
                    cd.PostId == commentEvent.PostId &&
                    cd.User?.Id == commentEvent.CommentAuthorId);

                await unitOfWork.CacheRepository<CachedCommentsNotification>()
                    .UpdateAsync(cacheUser);
            }

            // Save updated core data
            await unitOfWork.CoreRepository<Comments>()
                .UpdateAsync(authorNotification);

            await unitOfWork.SaveChangesAsync();



        }

    }
}
