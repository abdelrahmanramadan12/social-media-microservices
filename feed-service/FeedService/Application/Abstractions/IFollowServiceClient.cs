using Application.DTOs;

namespace Application.Abstractions
{
    internal interface IFollowServiceClient
    {
        Task<FollowsDTO> ListFollowersAsync(string userId);
    }
}
