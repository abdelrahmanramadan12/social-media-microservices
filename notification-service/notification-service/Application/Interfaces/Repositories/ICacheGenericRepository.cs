using Microsoft.EntityFrameworkCore.Query;
using MongoDB.Bson.IO;
using System.Linq.Expressions;

namespace Application.Interfaces.Repositories
{
    public interface ICacheGenericRepository<T> where T : class
    {

        Task AddAsync(T entity);
        Task DeleteAsync(T entity);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<IQueryable<T>> GetAll(string? userID = "");
        Task<T> GetAsync(string id, string? next , string? id2 = "", long number = 10);
        Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate);
        Task UpdateAsync(T entity, string? ID = "");
        public Task<T?> GetSingleByIdAsync(string userId);

    }
}
