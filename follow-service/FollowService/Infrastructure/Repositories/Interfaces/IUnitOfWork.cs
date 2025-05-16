using Domain.Entities;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }

        int Save();
        Task<int> SaveAsync();
    }
}