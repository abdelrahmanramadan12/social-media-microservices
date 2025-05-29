using Application.DTOs;
using Application.DTOs.Follow;

namespace Application.Services.Interfaces
{
    public interface IFollowServiceClient
    {
        // IsFollower()
        public Task<ServiceResponseDTO<bool>> IsFollower(IsFollowerRequest);


    }
}
