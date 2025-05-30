using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Query;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class CoreGenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        public CoreGenericRepository(IMongoClient mongoClient, string dbName, string collectionName)
        {
            var database = mongoClient.GetDatabase(dbName);
            _collection = database.GetCollection<T>(collectionName);
        }

        public async Task AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _collection.InsertManyAsync(entities);
        }

        public async Task DeleteAsync(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException("Entity must have an Id property.");

            var idValue = idProperty.GetValue(entity)?.ToString();
            var filter = Builders<T>.Filter.Eq("Id", idValue);
            await _collection.DeleteOneAsync(filter);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).AnyAsync();
        }

        public Task<IQueryable<T>> GetAll(string? userID = "", NotificationEntity notificationEntity = NotificationEntity.All)
        {
            return Task.FromResult(_collection.AsQueryable());
        }

        public Task<IEnumerable<T>> GetAllIncludingAsync(params Expression<Func<T, object>>[] includes)
        {
            return Task.FromResult(_collection.AsQueryable().AsEnumerable()); // Includes not applicable in MongoDB
        }

        public async Task<T?> GetAsync(int id, string? id2 = "", long number = 0)
        {
            var filter = Builders<T>.Filter.Eq("Id", id.ToString());
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<T?> GetAsync(string id, string? id2 = "", long number = 0)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).FirstOrDefaultAsync();
        }

        public async Task<T?> GetSingleIncludingAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            // Includes aren't relevant in MongoDB, just return the match
            return await _collection.Find(predicate).FirstOrDefaultAsync();
        }

        public async Task<T?> GetSingleDeepIncludingAsync(Expression<Func<T, bool>> predicate, params Func<IQueryable<T>, IQueryable<T>>[] includes)
        {
            return await _collection.Find(predicate).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(T entity, string? ID = "")
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException("Entity must have an Id property.");

            var idValue = idProperty.GetValue(entity)?.ToString();
            var filter = Builders<T>.Filter.Eq("Id", idValue);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public Task<IQueryable<T>> GetAll(string? userID = "")
        {
            throw new NotImplementedException();
        }

        IEnumerable<T> IGenericRepository<T>.GetAllIncludingAsync(params Expression<Func<T, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<T?> GetSingleDeepIncludingAsync(Expression<Func<T, bool>> predicate, params Func<IQueryable<T>, IIncludableQueryable<T, object>>[] includes)
        {
            throw new NotImplementedException();
        }
    }
}
