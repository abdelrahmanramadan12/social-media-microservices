using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {

        Task AddAsync(T Entity);
        Task DeleteAsync(T Entity);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<IQueryable<T>> GetAll(string? userID = "");
        public Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate);

        IEnumerable<T> GetAllIncludingAsync(params Expression<Func<T, object>>[] includes);
        Task<T?> GetAsync(string id, string? id2 = "", long number = 0);
        Task<T?> GetSingleDeepIncludingAsync(Expression<Func<T, bool>> predicate,
                                                          params Func<IQueryable<T>,
                                                          IIncludableQueryable<T, object>>[] includes);
        Task<T?> GetSingleIncludingAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task UpdateAsync(T Entity, string? ID = "");
        public Task AddRangeAsync(IEnumerable<T> entities);


    }
}
