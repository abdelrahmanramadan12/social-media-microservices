using Domain.Entities;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> FindAsync(string id);
        Task<User> AddAsync(User user);
        Task DeleteAsync(string id);
    }
}
