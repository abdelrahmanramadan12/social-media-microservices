using System;
using System.Threading.Tasks;
using Domain.Interfaces;
using Infrastructure.Context;
using Infrastructure.Mongodb;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StackExchange.Redis;

namespace Infrastructure.Repositories
{
    public class UnitOfWork(IMongoClient mongoClient, IConnectionMultiplexer redisConnection , IOptions<MongoDBSettings> mongoSettings) : IUnitOfWork
    {
        private readonly string _databaseName = mongoSettings.Value.DatabaseName;

        public ICoreGenericRepository<T> CoreRepository<T>() where T : class
        {

            // Use the class name of T as the collection name
            var collectionName = typeof(T).Name;

            return new CoreGenericRepository<T>(mongoClient,_databaseName, collectionName);
        }

        public ICacheGenericRepository<T> CacheRepository<T>() where T : class
        {
            return new CacheGenericRepository<T>(redisConnection);
        }

        public Task<int> SaveChangesAsync() => Task.FromResult(1);

    }
}