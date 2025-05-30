using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUnitOfWork
    {
        ICoreGenericRepository<T> CoreRepository<T>() where T : class;
        ICacheGenericRepository<T> CacheRepository<T>() where T : class;
        Task<int> SaveChangesAsync();
    }
}