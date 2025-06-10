using Application.DTOs.Responses;

namespace Application.Abstractions
{
    public interface IFollowServiceClient
    {
        Task<ResponseWrapper<List<string>>> ListFollowersAsync(string userId);
    }
}
