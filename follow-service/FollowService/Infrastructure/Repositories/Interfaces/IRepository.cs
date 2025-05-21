using System.Linq.Expressions;

namespace Infrastructure.Repositories.Interfaces
{
    public enum Order
    {
        ASC,
        DESC
    }

    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(string[]? includes = null, int? skip = null, int? take = null, Expression<Func<T, object>>? order = null, Order direction = Order.ASC);
        Task<IEnumerable<T>> GetAllAsync(string[]? includes = null, int? skip = null, int? take = null, Expression<Func<T, object>>? order = null, Order direction = Order.ASC);

        T Find(Expression<Func<T, bool>> expression, string[]? includes = null, Expression<Func<T, object>>? order = null, Order direction = Order.ASC);
        Task<T> FindAsync(Expression<Func<T, bool>> expression, string[]? includes = null, Expression<Func<T, object>>? order = null, Order direction = Order.ASC);
        IEnumerable<T> FindAll(Expression<Func<T, bool>> expression, string[]? includes = null, int? skip = null, int? take = null, Expression<Func<T, object>>? order = null, Order direction = Order.ASC);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> expression, string[]? includes = null, int? skip = null, int? take = null, Expression<Func<T, object>>? order = null, Order direction = Order.ASC);

        T Add(T entity);
        Task<T> AddAsync(T entity);
        IEnumerable<T> AddRange(IEnumerable<T> entities);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

        T Update(T entity);

        void Attach(T entity);
        void AttachRange(IEnumerable<T> entities);

        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
    }
}
