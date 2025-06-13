using Application.Abstractions;
using Application.DTOs;
using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repositories
{
    public class FeedRepository : IFeedRepository
    {
        private readonly IMongoCollection<Feed> _feeds;

        public FeedRepository(IMongoDatabase db)
        {
            _feeds = db.GetCollection<Feed>("feeds");
            EnsureIndexes();
        }

        public void EnsureIndexes()
        {
            var indexModels = new List<CreateIndexModel<Feed>>
            {
                // Index on userId
                new CreateIndexModel<Feed>(
                    Builders<Feed>.IndexKeys.Ascending(f => f.UserId),
                    new CreateIndexOptions { Name = "idx_userId" }),

                // Index on timeline.postId
                new CreateIndexModel<Feed>(
                    Builders<Feed>.IndexKeys.Ascending("timeline.postId"),
                    new CreateIndexOptions { Name = "idx_timeline_postId" }),

                // Index on timeline.authorProfile.id
                new CreateIndexModel<Feed>(
                    Builders<Feed>.IndexKeys.Ascending("timeline.authorProfile.id"),
                    new CreateIndexOptions { Name = "idx_timeline_authorId" })
            };

            _feeds.Indexes.CreateMany(indexModels);
        }

        public async Task<Response<Feed>> FindUserFeedAsync(string userId)
        {
            var filter = Builders<Feed>.Filter.Eq(f => f.UserId, userId);

            try
            {
                var feed = await _feeds.Find(filter).FirstOrDefaultAsync();
                if (feed == null)
                {
                    return new Response<Feed>()
                    {
                        Success = false,
                        Value = null,
                        Errors = [$"DB returned a null or invalid response"]
                    };
                } else
                {
                    return new Response<Feed>()
                    {
                        Success = true,
                        Value = feed,
                        Errors = []
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response<Feed>()
                {
                    Success = false,
                    Value = null,
                    Errors = [$"Unhandled exception: {ex.Message}"]
                };
            }
        }

        public async Task IncrementCommentsCountAsync(string postId, int number)
        {
            var filter = Builders<Feed>.Filter.ElemMatch(
                f => f.Timeline,
                p => p.PostId == postId
            );

            var update = Builders<Feed>.Update
                .Inc("timeline.$[post].commentsCount", number);

            var arrayFilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("post.postId", postId)
                )
            };

            await _feeds.UpdateManyAsync(
                filter,
                update,
                new UpdateOptions { ArrayFilters = arrayFilter }
            );
        }

        public async Task IncrementReactsCountAsync(string postId, int number)
        {
            var filter = Builders<Feed>.Filter.ElemMatch(
                f => f.Timeline,
                p => p.PostId == postId
            );

            var update = Builders<Feed>.Update
                .Inc("timeline.$[post].reactsCount", number);

            var arrayFilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("post.postId", postId)
                )
            };

            await _feeds.UpdateManyAsync(
                filter,
                update,
                new UpdateOptions { ArrayFilters = arrayFilter }
            );
        }

        public async Task PushToFeedAsync(Post post, string userId)
        {
            var update = Builders<Feed>.Update.PushEach(
                f => f.Timeline,
                [post],
                slice: -500
            );

            await _feeds.UpdateOneAsync(
                Builders<Feed>.Filter.Eq(f => f.UserId, userId),
                update,
                new UpdateOptions { IsUpsert = true }
            );
        }

        public async Task RemoveAuthorAsync(string userId)
        {
            var filter = Builders<Feed>.Filter.ElemMatch(
                f => f.Timeline,
                p => p.AuthorProfile.Id == userId
            );

            var update = Builders<Feed>.Update.PullFilter(
                f => f.Timeline,
                p => p.AuthorProfile.Id == userId
            );

            await _feeds.UpdateManyAsync(filter, update);
        }

        public async Task RemovePostAsync(string postId)
        {
            var filter = Builders<Feed>.Filter.ElemMatch(
                f => f.Timeline,
                p => p.PostId == postId
            );

            var update = Builders<Feed>.Update.PullFilter(
                f => f.Timeline,
                p => p.PostId == postId
            );

            await _feeds.UpdateManyAsync(filter, update);
        }

        public async Task RemoveUnfollowedPostsAsync(string followerId, string followingId)
        {
            var filter = Builders<Feed>.Filter.ElemMatch(
                f => f.Timeline,
                p => p.AuthorProfile.Id == followingId
            ) & Builders<Feed>.Filter.Eq(f => f.UserId, followerId);

            var update = Builders<Feed>.Update.PullFilter(
                f => f.Timeline,
                p => p.AuthorProfile.Id == followingId
            );

            await _feeds.UpdateManyAsync(filter, update);
        }

        public async Task SetLikedAsync(string userId, string postId, bool liked)
        {
            var filter = Builders<Feed>.Filter.And(
                Builders<Feed>.Filter.Eq(f => f.UserId, userId),
                Builders<Feed>.Filter.ElemMatch(f => f.Timeline, p => p.PostId == postId)
            );

            var update = Builders<Feed>.Update
                .Set("timeline.$[post].isLiked", liked);

            var arrayFilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("post.postId", postId)
                )
            };

            await _feeds.UpdateOneAsync(
                filter,
                update,
                new UpdateOptions { ArrayFilters = arrayFilter }
            );
        }

        public async Task UpdateAuthorAsync(AuthorProfile authorProfile)
        {
            var filter = Builders<Feed>.Filter.ElemMatch(
                f => f.Timeline,
                p => p.AuthorProfile.Id == authorProfile.Id
            );

            var update = Builders<Feed>.Update
                .Set("timeline.$[post].authorProfile", authorProfile);

            var arrayFilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("post.authorProfile.id", authorProfile.Id)
                )
            };

            await _feeds.UpdateManyAsync(
                filter,
                update,
                new UpdateOptions { ArrayFilters = arrayFilter }
            );
        }

        public async Task UpdateContentAsync(Post post)
        {
            var filter = Builders<Feed>.Filter.ElemMatch(
                f => f.Timeline,
                p => p.PostId == post.PostId
            );

            var update = Builders<Feed>.Update
                .Set("Timeline.$[post].content", post.Content)
                .Set("Timeline.$[post].isEdited", post.IsEdited)
                .Set("Timeline.$[post].mediaList", post.MediaList)
                .Set("Timeline.$[post].privacy", post.Privacy);

            var arrayFilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("post.postId", post.PostId)
                )
            };

            await _feeds.UpdateManyAsync(
                filter,
                update,
                new UpdateOptions { ArrayFilters = arrayFilter }
            );
        }
    }
}
