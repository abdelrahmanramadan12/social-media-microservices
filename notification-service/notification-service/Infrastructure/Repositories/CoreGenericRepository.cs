using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class CoreGenericRepository<T>(CoreContext dbContext) : IGenericRepository<T> where T : class
    {
        private readonly CoreContext _dbContext = dbContext;

        public async Task AddAsync(T Entity)
                     => await _dbContext.Set<T>().AddAsync(Entity);
        public async Task DeleteAsync(T Entity)
            => await Task.Run(() => _dbContext.Remove(Entity));
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
                                => await _dbContext.Set<T>().AnyAsync(predicate);
        public async Task<IQueryable<T>> GetAll(string? userID = "", NotificationEntity notificationEntity = NotificationEntity.All)
                                => await Task.Run(() => _dbContext.Set<T>().AsNoTracking());

        public Task<IQueryable<T>> GetAll(string? userID = "")
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAllIncludingAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return [.. query];
        }
        public async Task<T?> GetAsync(int id, string? id2 = "", long number = 0) => await _dbContext.FindAsync<T>(id);

        public Task<T?> GetAsync(string id, string? id2 = "", long number = 0)
        {
            throw new NotImplementedException();
        }

        public Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<T?> GetSingleDeepIncludingAsync(Expression<Func<T, bool>> predicate,
                                                            params Func<IQueryable<T>,
                                                            IIncludableQueryable<T, object>>[] includes)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            foreach (var include in includes)
            {
                query = include(query);
            }
            return await query.FirstOrDefaultAsync(predicate);
        }
        public async Task<T?> GetSingleIncludingAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync(predicate);
        }
        public Task UpdateAsync(T Entity, string? ID = "") =>
            Task.Run(() => _dbContext.Set<T>().Update(Entity));
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
            // Note: This doesn't call SaveChanges - follows your pattern of separate update
        }

    }
}
