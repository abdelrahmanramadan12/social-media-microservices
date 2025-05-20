using Domain.DTOs;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Services.Interfaces;
using System.Linq.Expressions;

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

        public async Task<ICollection<String>> ListFollowing(string userId)
        {
            var following = (await _unitOfWork.Follows.FindAllAsync(f => f.FollowerId == userId))?.Select(f => f.FollowingId)?.ToList()?? [];
            return following;
        }

        public async Task<ICollection<String>> ListFollowers(string userId)
        {
            var followers = (await _unitOfWork.Follows.FindAllAsync(f => f.FollowingId == userId))?.Select(f => f.FollowerId)?.ToList()?? [];
            return followers;
        }

        public async Task<FollowingPageDTO> ListFollowingPage(string userId, string cursor)
        {
            const int PageSize = 30;

            Expression<Func<Follow, bool>> filter = f => f.FollowerId == userId;

            if (!string.IsNullOrEmpty(cursor))
            {
                var nextUserId = AesEncryptionService.Decrypt(cursor);
                filter = f => f.FollowerId == userId && string.Compare(f.FollowingId, nextUserId) > 0;
            }

            var pageFollows = (await _unitOfWork.Follows.FindAllAsync(
                expression: filter,
                skip: 0,
                take: PageSize + 1,
                order: f => f.FollowingId,
                direction: Order.ASC
            )).ToList();

            var page = pageFollows.Take(PageSize).Select(f => f.FollowingId).ToList();
            string? newNextUserId = pageFollows.Count > PageSize ? AesEncryptionService.Encrypt(pageFollows[PageSize].FollowingId) : null;

            return new FollowingPageDTO
            {
                Following = page,
                Next = newNextUserId
            };
        }

        public async Task<FollowersPageDTO> ListFollowersPage(string userId, string cursor)
        {
            const int PageSize = 30;

            Expression<Func<Follow, bool>> filter = f => f.FollowingId == userId;

            if (!string.IsNullOrEmpty(cursor))
            {
                var nextUserId = AesEncryptionService.Decrypt(cursor);
                filter = f => f.FollowingId == userId && string.Compare(f.FollowerId, nextUserId) > 0;
            }

            var pageFollows = (await _unitOfWork.Follows.FindAllAsync(
                expression: filter,
                skip: 0,
                take: PageSize + 1,
                order: f => f.FollowerId,
                direction: Order.ASC
            )).ToList();

            var page = pageFollows.Take(PageSize).Select(f => f.FollowerId).ToList();
            string? newCursor = pageFollows.Count > PageSize ? AesEncryptionService.Encrypt(pageFollows[PageSize].FollowerId) : null;

            return new FollowersPageDTO
            {
                Followers = page,
                Next = newCursor
            };
        }
    }
}
