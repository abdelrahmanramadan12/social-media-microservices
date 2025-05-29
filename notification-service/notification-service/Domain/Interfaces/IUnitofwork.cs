using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<T> CoreRepository<T>() where T : class;
        IGenericRepository<T> CacheRepository<T>() where T : class;
        Task<int> SaveChangesAsync();
    }
}