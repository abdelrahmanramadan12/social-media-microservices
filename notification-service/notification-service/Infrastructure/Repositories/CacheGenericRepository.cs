using Application.Interfaces.Repositories;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class CacheGenericRepository<T>(IConnectionMultiplexer redisConnection) : ICacheGenericRepository<T> where T : class
    {
        private readonly IDatabase _redisDb = redisConnection.GetDatabase();
        private readonly string _collectionName = typeof(T).Name.ToLowerInvariant();
        private readonly TimeSpan _cacheDuration = TimeSpan.FromDays(7); // TTL

        private static readonly JsonSerializerSettings _serializerSettings = new()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        public async Task AddAsync(T entity)
        {
            var id = CacheGenericRepository<T>.GetEntityId(entity);
            var key = GetRedisKey(id);
            var serializedEntity = JsonConvert.SerializeObject(entity, _serializerSettings);

            await _redisDb.StringSetAsync(key, serializedEntity, _cacheDuration).ConfigureAwait(false);
            await _redisDb.SetAddAsync($"{_collectionName}", key).ConfigureAwait(false);
        }

        public async Task DeleteAsync(T entity)
        {
            var id = CacheGenericRepository<T>.GetEntityId(entity);
            var key = GetRedisKey(id);

            await _redisDb.KeyDeleteAsync(key).ConfigureAwait(false);
            await _redisDb.SetRemoveAsync($"{_collectionName}", key).ConfigureAwait(false);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            var keys = await _redisDb.SetMembersAsync($"{_collectionName}").ConfigureAwait(false);
            if (keys.Length == 0) return false;

            var values = await _redisDb.StringGetAsync(keys.Select(k => (RedisKey)(string)k).ToArray()).ConfigureAwait(false);

            var entities = values
                .Select(v => JsonConvert.DeserializeObject<T>(v, _serializerSettings))
                .Where(e => e != null)
                .ToList();

            return entities.Any(predicate.Compile());
        }

        public async Task<IQueryable<T>> GetAll(string? userID = "")
        {
            var keys = await _redisDb.SetMembersAsync($"{_collectionName}").ConfigureAwait(false);
            if (keys.Length == 0) return new List<T>().AsQueryable();

            var values = await _redisDb.StringGetAsync(keys.Select(k => (RedisKey)(string)k).ToArray())
                                                                                .ConfigureAwait(false);

            var entities = values
                .Select(v => JsonConvert.DeserializeObject<T>(v, _serializerSettings))
                .Where(e => e != null)
                .ToList();

            if (!string.IsNullOrEmpty(userID))
            {
                entities = entities
                    .Where(e =>
                        e?.GetType().GetProperty("UserId")?.GetValue(e)?.ToString() == userID)
                    .ToList();
            }

            return entities.AsQueryable();
        }

        public async Task<T?> GetAsync(string id, string? next ,string? id2 = "",long number = 0 )
        {
            var key = GetRedisKey(id.ToString());
            var serializedEntity = await _redisDb.StringGetAsync(key).ConfigureAwait(false);
            return serializedEntity.HasValue
                ? JsonConvert.DeserializeObject<T>(serializedEntity, _serializerSettings)
                : null;
        }

        public async Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate)
        {
            var all = await GetAll();
            return all.FirstOrDefault(predicate.Compile());
        }
        public async Task<T?> GetSingleByIdAsync(string userId)
        {
            var key = $"{_collectionName}:{userId}";
            var value = await _redisDb.StringGetAsync(key);

            if (value.IsNullOrEmpty) return default;

            return JsonConvert.DeserializeObject<T>(value);
        }

        public async Task UpdateAsync(T entity, string? ID = "")
        {
            var id = string.IsNullOrEmpty(ID) ? CacheGenericRepository<T>.GetEntityId(entity) : ID;
            var key = GetRedisKey(id);
            var serializedEntity = JsonConvert.SerializeObject(entity, _serializerSettings);

            await _redisDb.StringSetAsync(key, serializedEntity, _cacheDuration).ConfigureAwait(false);
            await _redisDb.SetAddAsync($"{_collectionName}", key).ConfigureAwait(false);
        }

        //public IEnumerable<T> GetAllIncludingAsync(params Expression<Func<T, object>>[] includes)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<T?> GetSingleDeepIncludingAsync(Expression<Func<T, bool>> predicate, params Func<IQueryable<T>, IIncludableQueryable<T, object>>[] includes)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<T?> GetSingleIncludingAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        //{
        //    throw new NotImplementedException();
        //}
        //public async Task<List<T>> GetWhereAsync(Expression<Func<T, bool>> predicate)
        //{
        //    var all = await GetAll();
        //    return [.. all.Where(predicate.Compile())];
        //}

        private string GetRedisKey(string id)
        {
            return $"{_collectionName}:{id}";
        }

        private static string GetEntityId(T entity)
        {
            var propsToCheck = new[] { "Id", "ID", "id", "UserId", "AuthorId" };
            foreach (var propName in propsToCheck)
            {
                var prop = typeof(T).GetProperty(propName);
                if (prop != null)
                {
                    var value = prop.GetValue(entity)?.ToString();
                    if (!string.IsNullOrEmpty(value))
                        return value;
                }
            }

            throw new InvalidOperationException($"Entity of type {typeof(T).Name} must have an identifier property like 'Id', 'UserId', or 'AuthorId'.");
        }

        //public Task AddRangeAsync(IEnumerable<T> entities)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
