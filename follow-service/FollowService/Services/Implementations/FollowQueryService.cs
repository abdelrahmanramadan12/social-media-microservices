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
            var follow = await _unitOfWork.Follows.FindAsync(userId, otherId);
            return follow != null;
        }

        public async Task<bool> IsFollower(string userId, string otherId)
        {
            var follow = await _unitOfWork.Follows.FindAsync(otherId, userId);
            return follow != null;
        }

        public async Task<FollowsDTO> ListFollowing(string userId)
        {
            var following = (await _unitOfWork.Follows.FindFollowingAsync(userId))?.Select(f => f.FollowingId)?.ToList() ?? [];
            var dto = new FollowsDTO
            {
                Follows = following
            };
            return dto;
        }

        public async Task<FollowsDTO> ListFollowers(string userId)
        {
            var followers = (await _unitOfWork.Follows.FindFollowersAsync(userId))?.Select(f => f.FollowerId)?.ToList() ?? [];
            var dto = new FollowsDTO
            {
                Follows = followers
            };
            return dto;
        }

        public async Task<FollowsPageDTO> ListFollowingPage(string userId, string? cursor)
        {
            const int PageSize = 20;

            var followingPage = (await _unitOfWork.Follows.FindFollowingAsync(
                userId,
                cursor,
                PageSize + 1
            )).ToList();

            var page = followingPage.Take(PageSize).Select(f => f.FollowingId).ToList();
            cursor = followingPage.Count > PageSize ? followingPage[PageSize].FollowingId : null;

            return new FollowsPageDTO
            {
                Follows = page,
                Next = cursor
            };
        }

        public async Task<FollowsPageDTO> ListFollowersPage(string userId, string? cursor)
        {
            const int PageSize = 20;

            var followersPage = (await _unitOfWork.Follows.FindFollowersAsync(
                userId,
                cursor,
                PageSize + 1
            )).ToList();

            var page = followersPage.Take(PageSize).Select(f => f.FollowerId).ToList();
            cursor = followersPage.Count > PageSize ? followersPage[PageSize].FollowerId : null;

            return new FollowsPageDTO
            {
                Follows = page,
                Next = cursor
            };
        }
    }
}
