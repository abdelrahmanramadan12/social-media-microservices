using Domain.DTOs;

namespace Services.Interfaces
{
    public interface IFollowQueryService
    {
        Task<bool> IsFollowing(string userId, string otherId);
        Task<bool> IsFollower(string userId, string otherId);
        Task<ICollection<String>> ListFollowing(string userId);
        Task<ICollection<String>> ListFollowers(string userId);
        Task<FollowingPageDTO> ListFollowingPage(string userId, string cursor);
        Task<FollowersPageDTO> ListFollowersPage(string userId, string cursor);
    }
}
