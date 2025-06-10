using Application.Hubs;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.CacheEntities;
using Domain.CacheEntities.Comments;
using Domain.CoreEntities;
using Domain.Events;
using Microsoft.AspNetCore.SignalR;

namespace Application.Services
{

    public class CommentNotificationService(IUnitOfWork unitOfWork, IHubContext<CommentNotificationHub> hubContext)
        : ICommentNotificationService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;
        private readonly IHubContext<CommentNotificationHub> _hubContext = hubContext;


        public async Task UpdatCommentListNotification(CommentEvent commentEvent)
        {
            var authorNotification = await unitOfWork.CoreRepository<Comments>()
                .GetSingleAsync(x => x.PostAuthorId == commentEvent.PostAuthorId);

            if (authorNotification == null)
                return;

            // Add the comment ID
            if (!authorNotification.UserID_CommentIds.TryGetValue(commentEvent.CommentorId, out var commentList))
            {
                commentList = [];
                authorNotification.UserID_CommentIds[commentEvent.CommentorId] = commentList;
            }
            commentList.Add(commentEvent.Id);

            // Broadcast to all users in the comment notification map
            foreach (var kvp in authorNotification.UserID_CommentIds)
            {
                var userId = kvp.Key;

                var cacheUser = await unitOfWork.CacheRepository<CachedCommentsNotification>()
                    .GetSingleByIdAsync(userId);

                if (cacheUser == null)
                {
                    cacheUser = new() { UserId = userId, CommnetDetails = [] };
                }

                cacheUser.CommnetDetails ??= new List<CommnetNotificationDetails>();
                cacheUser.CommnetDetails.Add(new CommnetNotificationDetails
                {
                    CommentId = commentEvent.Id,
                    PostId = commentEvent.PostId,
                    User = new UserSkeleton
                    {
                        Id = commentEvent.CommentorId,
                        Seen = false
                    }
                });

                await unitOfWork.CacheRepository<CachedCommentsNotification>()
                    .UpdateAsync(cacheUser);

                // SignalR send to each user
                await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveCommentNotification", new
                {
                    CommentId = commentEvent.Id,
                    commentEvent.PostId,
                    commentEvent.CommentorId
                });
            }

            await unitOfWork.CoreRepository<Comments>()
                .UpdateAsync(authorNotification);

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
            if (authorNotification.UserID_CommentIds.TryGetValue(commentEvent.CommentorId, out var commentList))
            {
                commentList.Remove(commentEvent.Id);

                // Clean up the entry if there are no more comments by this user
                if (commentList.Count == 0)
                    authorNotification.UserID_CommentIds.Remove(commentEvent.CommentorId);
            }

            // Update the user's cached comment notification
            var cacheUser = await unitOfWork.CacheRepository<CachedCommentsNotification>()
                .GetSingleByIdAsync(commentEvent.CommentorId);

            if (cacheUser != null)
            {
                cacheUser.CommnetDetails?.RemoveAll(cd =>
                    cd.CommentId == commentEvent.Id &&
                    cd.PostId == commentEvent.PostId &&
                    cd.User?.Id == commentEvent.CommentorId);

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
