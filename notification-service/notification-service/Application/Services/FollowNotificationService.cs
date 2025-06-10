using Application.Hubs;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Application.Services;
using Domain.CacheEntities;
using Domain.CoreEntities;
using Domain.Events;
using Microsoft.AspNetCore.SignalR;

namespace Application.Services
{
    public class FollowNotificationService(IUnitOfWork unitOfWork1, IHubContext<FollowNotificationHub> hubContext , IProfileServiceClient profileServiceClient) : IFollowNotificationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork1;
        private readonly IHubContext<FollowNotificationHub> _hubContext = hubContext;
        private readonly IProfileServiceClient profileServiceClient = profileServiceClient;

        public async Task UpdateFollowersListNotification(FollowEvent followedDTO)
        {
            // --- CORE (MongoDB) ---
            var coreUserFollower = await _unitOfWork.CoreRepository<Follows>().GetAsync(followedDTO.FollowingId);
            var isNewCore = false;

            if (coreUserFollower == null)
            {
                coreUserFollower = new Follows
                {
                    MyId = followedDTO.FollowingId,
                    FollowersId = new List<string> { followedDTO.FollowerId },
                    FollowsNotifReadByAuthor = new List<string>()
                };
                isNewCore = true;
            }
            else if (!coreUserFollower.FollowersId.Contains(followedDTO.FollowerId))
            {
                coreUserFollower.FollowersId.Add(followedDTO.FollowerId);
            }

            // --- CACHE: CachedFollowed ---
            var cacheUserFollower = await _unitOfWork.CacheRepository<CachedFollowed>()
                .GetSingleAsync(i => i.UserId == followedDTO.FollowingId);
            var isNewCache = false;

            if (cacheUserFollower == null)
            {
                cacheUserFollower = new CachedFollowed
                {
                    UserId = followedDTO.FollowingId,
                    Followers = new List<UserSkeleton>()
                };
                isNewCache = true;
            }

            // --- Check if follower already exists in cacheUserFollower followers list
            var existingFollowerInList = cacheUserFollower.Followers
                .FirstOrDefault(f => f.UserId == followedDTO.FollowerId);

            // --- GLOBAL CHECK: See if follower already exists in global UserSkeleton cache
            var existingGlobalUser = await _unitOfWork.CacheRepository<UserSkeleton>()
                .GetSingleAsync(u => u.UserId == followedDTO.FollowerId);
            UserSkeleton followerUser = new UserSkeleton();

            if (existingFollowerInList == null)
            {

                if (existingGlobalUser != null)
                {
                    // Use from cache
                    followerUser = existingGlobalUser;
                }
                else
                {
                    // Fetch from external service
                    var profile = await profileServiceClient.GetProfileAsync(followedDTO.FollowerId);
                    if (profile == null) return;

                    followerUser = new UserSkeleton
                    {
                        UserId = profile.Data.UserId,
                        UserNames = profile.Data.UserNames,
                        ProfileImageUrls = profile.Data.ProfileImageUrl,
                    };

                    await _unitOfWork.CacheRepository<UserSkeleton>().AddAsync(followerUser);
                }

                cacheUserFollower.Followers.Add(followerUser);
            }
            else
            {
                // Update follower info in list and global if exists
                existingFollowerInList.UserNames =  followerUser.UserNames ;
                existingFollowerInList.ProfileImageUrls = followerUser.ProfileImageUrls;

                await _unitOfWork.CacheRepository<UserSkeleton>().UpdateAsync(existingFollowerInList);
            }

            // --- Persist Core ---
            if (isNewCore)
                await _unitOfWork.CoreRepository<Follows>().AddAsync(coreUserFollower);
            else
                await _unitOfWork.CoreRepository<Follows>().UpdateAsync(coreUserFollower);

            // --- Persist Cache ---
            if (isNewCache)
                await _unitOfWork.CacheRepository<CachedFollowed>().AddAsync(cacheUserFollower);
            else
                await _unitOfWork.CacheRepository<CachedFollowed>().UpdateAsync(cacheUserFollower);


            // --- SignalR Push ---
            await _hubContext.Clients.User(followedDTO.FollowingId.ToString())
                .SendAsync("ReceiveFollowNotification", new
                {
                    followedDTO.FollowingId,
                    followerUser.UserNames,
                    followerUser.ProfileImageUrls,
                    Timestamp = DateTime.UtcNow
                });
        }
        public async Task RemoveFollowerFromNotificationList(FollowEvent followedDTO)
        {
            var coreUserFollower = await _unitOfWork.CoreRepository<Follows>().GetAsync(followedDTO.FollowingId);
            if (coreUserFollower == null)
                return;

            coreUserFollower.FollowersId.Remove(followedDTO.FollowerId);
            coreUserFollower.FollowsNotifReadByAuthor.Remove(followedDTO.FollowerId);

            var cacheUserFollower = await _unitOfWork.CacheRepository<CachedFollowed>().GetAsync(followedDTO.FollowingId);
            if (cacheUserFollower == null)
                return;

            cacheUserFollower.UserId = followedDTO.FollowingId;
            cacheUserFollower.Followers.RemoveAll(x => x.UserId == followedDTO.FollowerId);

            await _unitOfWork.CacheRepository<CachedFollowed>().UpdateAsync(cacheUserFollower);
            await _unitOfWork.CoreRepository<Follows>().UpdateAsync(coreUserFollower);
            await _unitOfWork.SaveChangesAsync();

        }
    }
}
