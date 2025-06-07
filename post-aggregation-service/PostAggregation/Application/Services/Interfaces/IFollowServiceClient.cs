using Application.DTOs;
using Application.DTOs.Follow;

namespace Application.Services.Interfaces
{
    public interface IFollowServiceClient
    {
        // IsFollower()
        public Task<ResponseWrapper<bool>> IsFollower(IsFollowerRequest request);
        public Task<ResponseWrapper<List<string>>> FilterFollowers(FilterFollowStatusRequest request);
        public Task<ResponseWrapper<List<string>>> FilterFollowing(FilterFollowStatusRequest request);
    }
}
