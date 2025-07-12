using Service.DTOs;
using Domain.Entities;
using Service.DTOs.Requests;
using Service.DTOs.Responses;

namespace Service.Interfaces.ProfileServices
{
    public interface IProfileService
    {
        Task<ResponseWrapper<Profile>> GetByUserIdAsync(string userId);
        Task<ResponseWrapper<Profile>> GetByUserNameAsync(string userName);
        Task<ResponseWrapper<SimpleUserDto>> GetByUserIdMinAsync(string userId);
        Task<ResponseWrapper<SimpleUserDto>> GetByUserNameMinAsync(string userName);
        Task<ResponseWrapper<List<SimpleUserDto>>> GetUsersByIdsAsync(List<string> userIds);
        Task<ResponseWrapper<Profile>> AddAsync(string userId, ProfileRequestDto profile);
        Task<ResponseWrapper<Profile>> UpdateAsync(string userId, ProfileRequestDto profile);
        Task<ResponseWrapper<bool>> DeleteAsync(string userId);
        Task<ResponseWrapper<List<SimpleUserDto>>> SearchByUserName(string query,int pageNumber);
    }
}
