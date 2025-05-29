using Application.Abstractions;
using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repositories
{
    public class FollowRepository : IFollowRepository
    {
        private readonly IMongoCollection<Follow> _follows;

        public FollowRepository(IMongoDatabase db)
        {
            _follows = db.GetCollection<Follow>("follows");
        }

        public async Task<Follow> FindAsync(string followerId, string followingId)
        {
            var filter = Builders<Follow>.Filter.Eq(f => f.FollowerId, followerId)
                & Builders<Follow>.Filter.Eq(f => f.FollowingId, followingId);

            var follow = await _follows.FindAsync(filter);
            return await follow.FirstOrDefaultAsync();
        }

        public async Task<ICollection<Follow>> FindFollowersAsync(string followingId, string? cursor = null, int? pageSize = null)
        {
            var filter = Builders<Follow>.Filter.Eq(f => f.FollowingId, followingId);

            if (!String.IsNullOrEmpty(cursor))
                filter &= Builders<Follow>.Filter.Gte(f => f.FollowerId, cursor);

            var follows = _follows.Find(filter).SortBy(f => f.FollowerId).Limit(pageSize);

            return await follows.ToListAsync();
        }

        public async Task<ICollection<Follow>> FindFollowingAsync(string followerId, string? cursor = null, int? pageSize = null)
        {
            var filter = Builders<Follow>.Filter.Eq(f => f.FollowerId, followerId);

            if (!String.IsNullOrEmpty(cursor))
                filter &= Builders<Follow>.Filter.Gte(f => f.FollowingId, cursor);

            var follows = _follows.Find(filter).SortBy(f => f.FollowingId).Limit(pageSize);

            return await follows.ToListAsync();
        }

        public async Task<Follow> AddAsync(Follow follow)
        {
            await _follows.InsertOneAsync(follow);
            return follow;
        }

        public async Task DeleteAsync(ObjectId id)
        {
            var filter = Builders<Follow>.Filter.Eq(f => f.Id, id);
            await _follows.DeleteOneAsync(filter);
            return;
        }
    }
}
