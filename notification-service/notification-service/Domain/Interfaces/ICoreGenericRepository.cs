using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Domain.Interfaces
{
    public interface ICoreGenericRepository<T> where T : class
    {

        Task AddAsync(T Entity);
        Task DeleteAsync(T Entity);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<IQueryable<T>> GetAll(string? userID = "");
        public Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<T>> GetAllIncludingAsync(params Expression<Func<T, object>>[] includes);
        Task<T?> GetAsync(string id, string? id2 = "", long number = 0);
       Task<T?> GetSingleIncludingAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task UpdateAsync(T Entity, string? ID = "");
        public Task AddRangeAsync(IEnumerable<T> entities);


    }
}
