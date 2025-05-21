using Domain.DTOs;
using Infrastructure.Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class FollowQueryService : IFollowQueryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FollowQueryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> IsFollowing(string userId, string otherId)
        {
            var follow = await _unitOfWork.Follows.FindAsync(f => f.FollowerId == userId && f.FollowingId == otherId);
            return follow != null;
        }

        public async Task<bool> IsFollower(string userId, string otherId)
        {
            var follow = await _unitOfWork.Follows.FindAsync(f => f.FollowerId == otherId && f.FollowingId == userId);
            return follow != null;
        }

        public async Task<FollowsDTO> ListFollowing(string userId)
        {
            var following = (await _unitOfWork.Follows.FindAllAsync(f => f.FollowerId == userId))?.Select(f => f.FollowingId)?.ToList() ?? [];
            var dto = new FollowsDTO
            {
                Follows = following
            };
            return dto;
        }

        public async Task<FollowsDTO> ListFollowers(string userId)
        {
            var followers = (await _unitOfWork.Follows.FindAllAsync(f => f.FollowingId == userId))?.Select(f => f.FollowerId)?.ToList() ?? [];
            var dto = new FollowsDTO
            {
                Follows = followers
            };
            return dto;
        }

        public async Task<FollowsPageDTO> ListFollowingPage(string userId, int? cursor)
        {
            const int PageSize = 30;

            var followingPage = (await _unitOfWork.Follows.FindAllAsync(
                f => f.FollowerId == userId,
                skip: cursor ?? 0,
                take: PageSize + 1,
                order: f => f.FollowingId,
                direction: Order.ASC
            )).ToList();

            var page = followingPage.Take(PageSize).Select(f => f.FollowerId).ToList();
            cursor = followingPage.Count > PageSize ? (cursor == null ? 0 : cursor) + PageSize : null;

            return new FollowsPageDTO
            {
                Follows = page,
                Next = cursor
            };
        }

        public async Task<FollowsPageDTO> ListFollowersPage(string userId, int? cursor)
        {
            const int PageSize = 30;

            var followersPage = (await _unitOfWork.Follows.FindAllAsync(
                f => f.FollowingId == userId,
                skip: cursor ?? 0,
                take: PageSize + 1,
                order: f => f.FollowerId,
                direction: Order.ASC
            )).ToList();

            var page = followersPage.Take(PageSize).Select(f => f.FollowerId).ToList();
            cursor = followersPage.Count > PageSize ? (cursor == null ? 0 : cursor) + PageSize : null;

            return new FollowsPageDTO
            {
                Follows = page,
                Next = cursor
            };
        }
    }
}
