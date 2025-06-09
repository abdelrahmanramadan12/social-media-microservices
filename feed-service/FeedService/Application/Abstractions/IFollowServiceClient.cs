using Application.DTOs;

namespace Application.Abstractions
{
    public interface IFollowServiceClient
    {
        Task<Response<FollowsDTO>> ListFollowersAsync(string userId);
    }
}
