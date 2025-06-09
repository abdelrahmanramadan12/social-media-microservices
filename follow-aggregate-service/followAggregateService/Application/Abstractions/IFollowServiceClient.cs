using Application.DTOs;

namespace Application.Abstractions
{
    public interface IFollowServiceClient
    {
        Task<Response<FollowsPageDTO>> ListFollowersPage(string userId, string? cursor);
        Task<Response<FollowsPageDTO>> ListFollowingPage(string userId, string? cursor);
    }
}
