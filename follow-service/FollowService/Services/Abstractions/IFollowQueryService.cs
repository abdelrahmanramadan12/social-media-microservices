using Application.DTOs;

namespace Application.Abstractions
{
    public interface IFollowQueryService
    {
        Task<bool> IsFollowing(string userId, string otherId);
        Task<bool> IsFollower(string userId, string otherId);
        Task<FollowsDTO> ListFollowing(string userId);
        Task<FollowsDTO> ListFollowers(string userId);
        Task<FollowsPageDTO> ListFollowingPage(string userId, string? cursor);
        Task<FollowsPageDTO> ListFollowersPage(string userId, string? cursor);
        Task<List<string>> FilterFollowers(string userId, List<string> otherIds);
        Task<List<string>> FilterFollowings(string userId, List<string> otherIds);
    }
}
