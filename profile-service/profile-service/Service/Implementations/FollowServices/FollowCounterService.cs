using Domain.Events;
using Domain.IRepository;
using Service.Interfaces.FollowServices;

namespace Service.Implementations.FollowServices
{
    public class FollowCounterService :IFollowCounterService
    {
        private readonly IProfileRepository _profileRepository;

        public FollowCounterService(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        
        public async Task UpdateCounter(string followerId ,string followedId, FollowEventType followType)
        {
            if (followType == FollowEventType.Follow)
            {
                await _profileRepository.IncrementFollowingCountAsync(followerId);
                await _profileRepository.IncrementFollowerCountAsync(followedId);
            }
            else if (followType == FollowEventType.Unfollow)
            {
                await _profileRepository.DecrementFollowingCountAsync(followerId);
                await _profileRepository.DecrementFollowerCountAsync(followedId);
            }

        }



    }
}
