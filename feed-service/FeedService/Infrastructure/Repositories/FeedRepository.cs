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
                .Inc("Timeline.$[post].CommentsCount", number);

            var arrayFilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("post.PostId", postId)
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
                .Inc("Timeline.$[post].ReactsCount", number);

            var arrayFilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("post.PostId", postId)
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
                .Set("Timeline.$[post].IsLiked", liked);

            var arrayFilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("post.PostId", postId)
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
                .Set("Timeline.$[post].AuthorProfile", authorProfile);

            var arrayFilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("post.AuthorProfile.Id", authorProfile.Id)
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
                .Set("Timeline.$[post].Content", post.Content)
                .Set("Timeline.$[post].IsEdited", post.IsEdited)
                .Set("Timeline.$[post].MediaList", post.MediaList)
                .Set("Timeline.$[post].Privacy", post.Privacy);

            var arrayFilter = new[]
            {
                new BsonDocumentArrayFilterDefinition<BsonDocument>(
                    new BsonDocument("post.PostId", post.PostId)
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
