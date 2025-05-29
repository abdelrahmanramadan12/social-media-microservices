using Domain.Entities;

namespace Application.Abstractions
{
    public interface IUserRepository
    {
        Task<User> FindAsync(string id);
        Task<User> AddAsync(User user);
        Task DeleteAsync(string id);
    }
}
