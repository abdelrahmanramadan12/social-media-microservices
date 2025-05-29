using System;
using System.Threading.Tasks;
using Domain.Interfaces;
using Infrastructure.Context;
using StackExchange.Redis;

namespace Infrastructure.Repositories
{
    public class UnitOfWork(CoreContext dbContext, IConnectionMultiplexer redisConnection) : IUnitOfWork, IDisposable
    {
        private readonly CoreContext _dbContext = dbContext;
        private readonly IConnectionMultiplexer _redisConnection = redisConnection;

        public IGenericRepository<T> CoreRepository<T>() where T : class
        {
            return new CoreGenericRepository<T>(_dbContext);
        }

        public IGenericRepository<T> CacheRepository<T>() where T : class
        {
            return new CacheGenericRepository<T>(_redisConnection);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
             _redisConnection.Dispose(); // Only if you own the connection's lifecycle
        }
    }
}