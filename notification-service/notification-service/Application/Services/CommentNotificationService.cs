using Application.Interfaces.Services;
using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Events;
using Application.Interfaces.Services;
using Domain.CoreEntities;
using Domain.CacheEntities.Comments;
using Domain.CacheEntities;

namespace Application.Services
{
    internal class CommentNotificationService(IUnitOfWork unitOfWork) : ICommentNotificationService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;

       public async Task UpdatCommentListNotification(CommentEvent CommentEvent)
        {

            var user = await unitOfWork.CoreRepository<Comment>().GetSingleIncludingAsync(i => i.AuthorId == CommentEvent.PostAuthorId);
            if (user == null)
                return;
            user.UserID_CommentId.Add(new Dictionary<string, string>
            {
                { CommentEvent.CommentorId, CommentEvent.Id }
            });     
            await unitOfWork.CoreRepository<Comment>().UpdateAsync(user);
            

            var cacheUser = await unitOfWork.CacheRepository<CachedComments>().GetSingleByIdAsync(CommentEvent.CommentorId);
            if (cacheUser == null)
                return;
            cacheUser.CommnetDetails.Add(new CommnetDetails
            {
                CommentId = CommentEvent.Id,
                PostId = CommentEvent.PostId,
                User = new UserSkeleton
                {
                    Id = CommentEvent.CommentorId,
                    Seen = false,
                }
            });
            await unitOfWork.CacheRepository<CachedComments>().UpdateAsync(cacheUser);



        }
    }
}
