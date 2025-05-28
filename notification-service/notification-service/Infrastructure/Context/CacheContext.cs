using Domain.CacheEntities;
using Domain.CacheEntities.Comments;
using Domain.CacheEntities.Reactions;
using Infrastructure.Mongodb;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context
{
    public class CacheContext
    {
        private readonly IMongoDatabase _database;

        public CacheContext(IOptions<MongoDBSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        // Collections
        public IMongoCollection<CachedFollowed> Followed => _database.GetCollection<CachedFollowed>("followed");
        public IMongoCollection<CachedReactions> Reactions => _database.GetCollection<CachedReactions>("reactions");
        public IMongoCollection<CachedComments> Comments => _database.GetCollection<CachedComments>("comments");
    }
}
