using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Infrastructure.Data;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        private readonly FollowDbContext _context;

        public BaseRepository(FollowDbContext context)
        {
            _context = context;
        }

        public T Get(int id)
        {
            return _context.Set<T>().Find(id);
        }
        public async Task<T> GetAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
        public IEnumerable<T> GetAll(
            string[]? includes = null,
            int? skip = null,
            int? take = null,
            Expression<Func<T, object>>? order = null,
            Order direction = Order.ASC)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (order != null)
            {
                query = direction == Order.ASC
                    ? query.OrderBy(order)
                    : query.OrderByDescending(order);
            }

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            return query.ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            string[]? includes = null,
            int? skip = null,
            int? take = null,
            Expression<Func<T, object>>? order = null,
            Order direction = Order.ASC)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (order != null)
            {
                query = direction == Order.ASC
                    ? query.OrderBy(order)
                    : query.OrderByDescending(order);
            }

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public T Find(Expression<Func<T, bool>> expression, string[]? includes = null, Expression<Func<T, object>>? order = null, Order direction = Order.ASC)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            if (order != null)
            {
                if (direction == Order.ASC)
                    query = query.OrderBy(order);
                else
                    query = query.OrderByDescending(order);
            }

            return query.SingleOrDefault(expression);
        }
        public async Task<T> FindAsync(Expression<Func<T, bool>> expression, string[]? includes = null, Expression<Func<T, object>>? order = null, Order direction = Order.ASC)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            if (order != null)
            {
                if (direction == Order.ASC)
                    query = query.OrderBy(order);
                else
                    query = query.OrderByDescending(order);
            }

            return await query.SingleOrDefaultAsync(expression);
        }
        public IEnumerable<T> FindAll(
            Expression<Func<T, bool>> expression,
            string[]? includes = null,
            int? skip = null,
            int? take = null,
            Expression<Func<T, object>>? order = null,
            Order direction = Order.ASC)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            query = query.Where(expression);

            if (order != null)
            {
                query = direction == Order.ASC ? query.OrderBy(order) : query.OrderByDescending(order);
            }

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);
            
            return query.ToList();
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> expression,string[]? includes = null,int? skip = null, int? take = null,Expression<Func<T, object>>? order = null,Order direction = Order.ASC)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            query = query.Where(expression);

            if (order != null)
            {
                query = direction == Order.ASC
                    ? query.OrderBy(order)
                    : query.OrderByDescending(order);
            }

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public T Add(T entity)
        {
            _context.Set<T>().Add(entity);
            return entity;
        }
        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }
        public IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
            return entities;
        }
        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
            return entities;
        }

        public T Update(T entity)
        {
            _context.Update(entity);
            return entity;
        }

        public void Attach(T entity)
        {
            _context.Set<T>().Attach(entity);
        }
        public void AttachRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AttachRange(entities);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }
    }
}
