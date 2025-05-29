using Domain.DTOs;
using Domain.Entities;

namespace Service.Interfaces.ProfileServices
{
    public interface IProfileService
    {
        Task<ProfileResponseDto?> GetByUserIdAsync(string userId);
        Task<ProfileResponseDto?> GetByUserNameAsync(string userName);
        Task<MinProfileResponseDto?> GetByUserIdMinAsync(string userId);
        Task<MinProfileResponseDto?> GetByUserNameMinAsync(string userName);
        Task<ProfileListResponseDto?> GetUsersByIdsAsync(List<string> userIds);
        Task<ProfileResponseDto?> AddAsync(Profile profile);
        Task<ProfileResponseDto?> UpdateAsync(string userId, Profile profile);
        Task<bool> DeleteAsync(string userId);
    }
}
