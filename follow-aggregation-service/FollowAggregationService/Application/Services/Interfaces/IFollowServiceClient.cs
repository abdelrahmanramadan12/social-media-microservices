using Application.DTOs;
using Application.DTOs.Follow;

namespace Application.Services.Interfaces
{
    public interface IFollowServiceClient
    {
        Task<PaginationResponseWrapper<List<string>>> GetFollowers(ListFollowPageRequest request);
        Task<PaginationResponseWrapper<List<string>>> GetFollowing(ListFollowPageRequest request);
        public Task<ResponseWrapper<List<string>>> FilterFollowers(FilterFollowStatusRequest request);
        public Task<ResponseWrapper<List<string>>> FilterFollowing(FilterFollowStatusRequest request);
    }
}
