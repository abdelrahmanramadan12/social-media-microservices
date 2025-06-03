using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.CacheEntities;
using Domain.CoreEntities;
using Domain.Events;

namespace Application.Services
{
    public class FollowNotificationService(IUnitOfWork unitOfWork1) : IFollowNotificationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork1;
        public async Task UpdateFollowersListNotification(FollowEvent followedDTO)
        {
            var CoreUsersFollower = _unitOfWork.CoreRepository<Follows>().GetAsync(followedDTO.UserId);
            if (CoreUsersFollower == null)
                return;
            var CoreUsersFollowerResult = await CoreUsersFollower;
            if (CoreUsersFollowerResult == null)
                return;

            CoreUsersFollowerResult.FollowersId.Add(followedDTO.FollowerId);

            // Update the notification list for the user who is being followed
            if (!CoreUsersFollowerResult.FollowsNotifReadByAuthor.Contains(followedDTO.FollowerId))
                CoreUsersFollowerResult.FollowsNotifReadByAuthor.Add(followedDTO.FollowerId);


            var CacheUsersFollower = _unitOfWork.CacheRepository<CachedFollowed>().GetAsync(followedDTO.UserId).Result;
            if (CacheUsersFollower == null)
                return;
            CacheUsersFollower.UserId = followedDTO.UserId;
            CacheUsersFollower.Followers.Add(
                new UserSkeleton
                {
                    CreatedAt = DateTime.UtcNow,
                    ProfileImageUrls = followedDTO.ProfileImageUrls,
                    UserNames = followedDTO.UserNames,
                    UserId = followedDTO.FollowerId,
                    Seen = false
                });

            //save changes to the cache repository
            await _unitOfWork.CacheRepository<CachedFollowed>().UpdateAsync(CacheUsersFollower);
            await _unitOfWork.CoreRepository<Follows>().UpdateAsync(CoreUsersFollowerResult);
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task RemoveFollowerFromNotificationList(FollowEvent followedDTO)
        {
            var CoreUsersFollower = _unitOfWork.CoreRepository<Follows>().GetAsync(followedDTO.UserId);
            if (CoreUsersFollower == null)
                return;
            var CoreUsersFollowerResult = await CoreUsersFollower;
            if (CoreUsersFollowerResult == null)
                return;

            CoreUsersFollowerResult.FollowersId.Remove(followedDTO.FollowerId);
            // Update the notification list for the user who is being followed
            if (CoreUsersFollowerResult.FollowsNotifReadByAuthor.Contains(followedDTO.FollowerId))
                CoreUsersFollowerResult.FollowsNotifReadByAuthor.Remove(followedDTO.FollowerId);

            var CacheUsersFollower = _unitOfWork.CacheRepository<CachedFollowed>().GetAsync(followedDTO.UserId).Result;
            if (CacheUsersFollower == null)
                return;

            CacheUsersFollower.UserId = followedDTO.UserId;
            CacheUsersFollower.Followers.RemoveAll(x => x.UserId == followedDTO.FollowerId);
            //save changes to the cache repository
            await _unitOfWork.CacheRepository<CachedFollowed>().UpdateAsync(CacheUsersFollower);
            await _unitOfWork.CoreRepository<Follows>().UpdateAsync(CoreUsersFollowerResult);
            await _unitOfWork.SaveChangesAsync();

        }

    }
}
