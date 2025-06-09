using Domain.Entities;
using Application.Abstractions;
using Application.Events;

namespace Application.Implementations
{
    public class FollowCommandService : IFollowCommandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueuePublisher<FollowEvent> _followPublisher;

        public FollowCommandService(IUnitOfWork unitOfWork, IQueuePublisher<FollowEvent> followPublisher)
        {
            _unitOfWork = unitOfWork;
            _followPublisher = followPublisher;
        }

        public async Task<bool> Follow(string userId, string otherId)
        {
            if (userId == otherId)
            {
                return false;
            }

            var user = await _unitOfWork.Users.FindAsync(userId);
            var other = await _unitOfWork.Users.FindAsync(otherId);

            if (user == null || other == null)
            {
                return false;
            }

            var follow = await _unitOfWork.Follows.FindAsync(userId, otherId);

            if (follow == null)
            {
                follow = new Follow
                {
                    FollowerId = userId,
                    FollowingId = otherId,
                    FollowedAt = DateTime.UtcNow
                };

                await _unitOfWork.Follows.AddAsync(follow);

                var args = new FollowEvent
                {
                    EventType = FollowEventType.Follow,
                    FollowerId = follow.FollowerId,
                    FollowingId = follow.FollowingId,
                    Timestamp = follow.FollowedAt
                };

                //await _followPublisher.PublishAsync(args);
            }

            return true;
        }

        public async Task Unfollow(string userId, string otherId)
        {
            var follow = await _unitOfWork.Follows.FindAsync(userId, otherId);

            if (follow != null)
            {
                await _unitOfWork.Follows.DeleteAsync(follow.Id);

                var args = new FollowEvent
                {
                    EventType = FollowEventType.Unfollow,
                    FollowerId = follow.FollowerId,
                    FollowingId = follow.FollowingId,
                    Timestamp = DateTime.UtcNow
                };

                //await _followPublisher.PublishAsync(args);
            }
        }
    }
}
