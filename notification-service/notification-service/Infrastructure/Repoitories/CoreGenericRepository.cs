using Application.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repoitories
{
    public class CoreGenericRepository<T>(CoreContext dbContext) : IGenericRepository<T> where T : class
    {
        private readonly CoreContext _dbContext = dbContext;

        public async void AddAsync(T Entity)
                     => await _dbContext.Set<T>().AddAsync(Entity);
        public void DeleteAsync(T Entity) => _dbContext.Remove(Entity);
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
                                => await _dbContext.Set<T>().AnyAsync(predicate);
        public IQueryable<T> GetAll(string? userID = "", int flag = 1)
                                    => _dbContext.Set<T>().AsNoTracking();
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
        public void UpdateAsync(T Entity, string? ID = "") => _dbContext.Set<T>().Update(Entity);

    }
}
