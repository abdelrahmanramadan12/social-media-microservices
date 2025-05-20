using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class FollowCommandService : IFollowCommandService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FollowCommandService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Follow(string userId, string otherId)
        {
            if (userId == otherId)
            {
                return false;
            }

            var user = await _unitOfWork.Users.FindAsync(u => u.Id == userId);
            var other = await _unitOfWork.Users.FindAsync(u => u.Id == otherId);

            if (user == null || other == null)
            {
                return false;
            }

            var follow = await _unitOfWork.Follows.FindAsync(f => f.FollowerId == userId && f.FollowingId == otherId);

            if (follow == null)
            {
                follow = new Follow
                {
                    FollowerId = userId,
                    FollowingId = otherId,
                    FollowedAt = DateTime.UtcNow
                };

                await _unitOfWork.Follows.AddAsync(follow);
                await _unitOfWork.SaveAsync();
            }

            return true;
        }

        public async Task Unfollow(string userId, string otherId)
        {
            var follow = await _unitOfWork.Follows.FindAsync(f => f.FollowerId == userId && f.FollowingId == otherId);

            if (follow != null)
            {
                _unitOfWork.Follows.Delete(follow);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
