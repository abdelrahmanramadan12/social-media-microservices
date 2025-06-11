using Application.DTOs;
using Application.DTOs.Profile;

namespace Application.Services.Interfaces
{
    public interface IProfileServiceClient
    {
        Task<ResponseWrapper<SimpleUserProfile>> GetByUserIdMinAsync(string userId);
        Task<ResponseWrapper<List<SimpleUserProfile>>> GetUsersByIdsAsync(GetUsersProfileByIdsRequest request);
    }
}
