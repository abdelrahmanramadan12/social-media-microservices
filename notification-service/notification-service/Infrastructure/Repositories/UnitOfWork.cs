using System;
using System.Threading.Tasks;
using Domain.Interfaces;
using Infrastructure.Context;
using MongoDB.Driver;
using StackExchange.Redis;

namespace Infrastructure.Repositories
{
    public class UnitOfWork(IMongoClient mongoClient, IConnectionMultiplexer redisConnection) : IUnitOfWork
    {
        public IGenericRepository<T> CoreRepository<T>() where T : class
        {
            // Use the class name of T as the collection name
            var collectionName = typeof(T).Name;
            return new CoreGenericRepository<T>(mongoClient, "YourDbName", collectionName);
        }

        public IGenericRepository<T> CacheRepository<T>() where T : class
        {
            return new CacheGenericRepository<T>(redisConnection);
        }

        public Task<int> SaveChangesAsync() => Task.FromResult(1);

    }
}