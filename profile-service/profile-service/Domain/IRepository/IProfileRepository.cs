using Domain.DTOs;
using Domain.Entities;

namespace Domain.IRepository
{
    public interface IProfileRepository
    {
        Task<Profile?> GetByUserIdAsync(string userId);
        Task<Profile?> GetByUserNameAsync(string userName);
        Task<SimpleUserDto?> GetByUserIdMinAsync(string userId);
        Task<SimpleUserDto?> GetByUserNameMinAsync(string userName);
        Task<List<SimpleUserDto>> GetUsersByIdsAsync(List<string> userIds);
        Task AddAsync(Profile profile);
        Task Update(Profile profile);
        Task<bool> ExistsByUserIdAsync(string userId);
        Task DeleteAsync(string userId);


        Task IncrementFollowingCountAsync(string userId);
        Task IncrementFollowerCountAsync(string userId);
        Task DecrementFollowingCountAsync(string userId);
        Task DecrementFollowerCountAsync(string userId);
    }

}
