using Domain.DTOs;

namespace Services.Interfaces
{
    public interface IFollowQueryService
    {
        Task<bool> IsFollowing(string userId, string otherId);
        Task<bool> IsFollower(string userId, string otherId);
        Task<FollowsDTO> ListFollowing(string userId);
        Task<FollowsDTO> ListFollowers(string userId);
        Task<FollowsPageDTO> ListFollowingPage(string userId, int? cursor);
        Task<FollowsPageDTO> ListFollowersPage(string userId, int? cursor);
    }
}
