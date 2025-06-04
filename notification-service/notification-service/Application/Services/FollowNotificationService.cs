using Application.Hubs;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.CacheEntities;
using Domain.CoreEntities;
using Domain.Events;
using Application.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Application.Services
{
    public class FollowNotificationService(IUnitOfWork unitOfWork1, IHubContext<FollowNotificationHub> hubContext) : IFollowNotificationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork1;
        private readonly IHubContext<FollowNotificationHub> _hubContext = hubContext;

        public async Task UpdateFollowersListNotification(FollowEvent followedDTO)
        {
            var coreUserFollower = await _unitOfWork.CoreRepository<Follows>().GetAsync(followedDTO.UserId);
            if (coreUserFollower == null)
                return;

            coreUserFollower.FollowersId.Add(followedDTO.FollowerId);

            if (!coreUserFollower.FollowsNotifReadByAuthor.Contains(followedDTO.FollowerId))
                coreUserFollower.FollowsNotifReadByAuthor.Add(followedDTO.FollowerId);

            var cacheUserFollower = await _unitOfWork.CacheRepository<CachedFollowed>().GetAsync(followedDTO.UserId);
            if (cacheUserFollower == null)
                return;

            cacheUserFollower.UserId = followedDTO.UserId;
            cacheUserFollower.Followers.Add(new UserSkeleton
            {
                CreatedAt = DateTime.UtcNow,
                ProfileImageUrls = followedDTO.ProfileImageUrls,
                UserNames = followedDTO.UserNames,
                UserId = followedDTO.FollowerId,
                Seen = false
            });

            await _unitOfWork.CacheRepository<CachedFollowed>().UpdateAsync(cacheUserFollower);
            await _unitOfWork.CoreRepository<Follows>().UpdateAsync(coreUserFollower);
            await _unitOfWork.SaveChangesAsync();

            // SignalR push
            await _hubContext.Clients.User(followedDTO.UserId.ToString())
                .SendAsync("ReceiveFollowNotification", new
                {
                    FollowerId = followedDTO.FollowerId,
                    UserNames = followedDTO.UserNames,
                    ProfileImageUrls = followedDTO.ProfileImageUrls,
                    Timestamp = DateTime.UtcNow
                });
        }

        public async Task RemoveFollowerFromNotificationList(FollowEvent followedDTO)
        {
            var coreUserFollower = await _unitOfWork.CoreRepository<Follows>().GetAsync(followedDTO.UserId);
            if (coreUserFollower == null)
                return;

            coreUserFollower.FollowersId.Remove(followedDTO.FollowerId);
            coreUserFollower.FollowsNotifReadByAuthor.Remove(followedDTO.FollowerId);

            var cacheUserFollower = await _unitOfWork.CacheRepository<CachedFollowed>().GetAsync(followedDTO.UserId);
            if (cacheUserFollower == null)
                return;

            cacheUserFollower.UserId = followedDTO.UserId;
            cacheUserFollower.Followers.RemoveAll(x => x.UserId == followedDTO.FollowerId);

            await _unitOfWork.CacheRepository<CachedFollowed>().UpdateAsync(cacheUserFollower);
            await _unitOfWork.CoreRepository<Follows>().UpdateAsync(coreUserFollower);
            await _unitOfWork.SaveChangesAsync();

        }
    }
}
