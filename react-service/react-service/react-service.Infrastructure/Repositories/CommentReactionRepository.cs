using Microsoft.Extensions.Options;
using MongoDB.Driver;
using react_service.Domain.Entites;
using react_service.Infrastructure.Mongodb;
using react_service.Application.Interfaces.Repositories;
using react_service.Application.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace react_service.Infrastructure.Repositories
{
    public class CommentReactionRepository : ICommentReactionRepository
    {
        private readonly IMongoCollection<CommentReaction> _collection;
        private readonly IMongoClient mongoClient;
        public IOptions<PaginationSettings> PaginationSetting { get; }

        public CommentReactionRepository(IOptions<MongoDbSettings> mongodbsettings, IMongoClient mongoClient, IOptions<PaginationSettings> paginationSetting)
        {
            var database = mongoClient.GetDatabase(mongodbsettings.Value.ReactionsDatabaseName);
            _collection = database.GetCollection<CommentReaction>("CommentReaction");
            this.mongoClient = mongoClient;
            PaginationSetting = paginationSetting;
            // Optionally create indexes here
        }

        public Task<List<string>> FilterCommentsReactedByUserAsync(List<string> commentIds, string userId)
        {
            var filter = Builders<CommentReaction>.Filter.And(
                Builders<CommentReaction>.Filter.In(r => r.CommentId, commentIds),
                Builders<CommentReaction>.Filter.Eq(r => r.UserId, userId),
                Builders<CommentReaction>.Filter.Eq(r => r.IsDeleted, false)
            );
            return _collection.Find(filter)
                .Project(r => r.CommentId)
                .ToListAsync();
        }

        public async Task<List<CommentReaction>> GetReactsOfCommentAsync(string commentId, string nextReactIdHash)
        {
            var filterBuilder = Builders<CommentReaction>.Filter;
            var filters = new List<FilterDefinition<CommentReaction>>
            {
                filterBuilder.Eq(r => r.CommentId, commentId),
                filterBuilder.Eq(r => r.IsDeleted, false)
            };
            if (!string.IsNullOrEmpty(nextReactIdHash))
            {
                filters.Add(filterBuilder.Lt(r => r.Id, nextReactIdHash));
            }
            var filter = filterBuilder.And(filters);
            return await _collection
                .Find(filter)
                .SortByDescending(r => r.CreatedAt)
                .Limit(PaginationSetting.Value.DefaultPageSize)
                .ToListAsync();
        }

        public async Task<List<CommentReaction>> GetCommentsReactedByUserAsync(string userId, string nextReactIdHash)
        {
            var filterBuilder = Builders<CommentReaction>.Filter;
            var filters = new List<FilterDefinition<CommentReaction>>
            {
                filterBuilder.Eq(r => r.UserId, userId),
                filterBuilder.Eq(r => r.IsDeleted, false)
            };
            if (!string.IsNullOrEmpty(nextReactIdHash))
            {
                filters.Add(filterBuilder.Lt(r => r.Id, nextReactIdHash));
            }
            var filter = filterBuilder.And(filters);
            return await _collection
                .Find(filter)
                .SortByDescending(r => r.CreatedAt)
                .Limit(PaginationSetting.Value.DefaultPageSize)
                .ToListAsync();
        }

        public async Task<bool> DeleteReactionAsync(string commentId, string userId)
        {
            var filter = Builders<CommentReaction>.Filter.And(
                Builders<CommentReaction>.Filter.Eq(r => r.CommentId, commentId),
                Builders<CommentReaction>.Filter.Eq(r => r.UserId, userId)
            );
            var update = Builders<CommentReaction>.Update.Set(r => r.IsDeleted, true);
            var result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<string> AddReactionAsync(CommentReaction reaction)
        {
            await _collection.InsertOneAsync(reaction);

            return reaction.CommentId;
        }

        public async Task<bool> DeleteAllCommentReactions(string commentId)
        {
            var filter = Builders<CommentReaction>.Filter.Eq(r => r.CommentId, commentId);
            var update = Builders<CommentReaction>.Update.Set(r => r.IsDeleted, true);
            var result = await _collection.UpdateManyAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<List<string>> GetUserIdsReactedToCommentAsync(string commentId)
        {
            var filter = Builders<CommentReaction>.Filter.And(
                Builders<CommentReaction>.Filter.Eq(r => r.CommentId, commentId),
                Builders<CommentReaction>.Filter.Eq(r => r.IsDeleted, false)
            );
            return await _collection.Find(filter)
                .Project(r => r.UserId)
                .ToListAsync();
        }

        public async Task<List<string>> GetUserIdsReactedToCommentAsync(string commentId, string lastSeenId, int take)
        {
            var filterBuilder = Builders<CommentReaction>.Filter;
            var filters = new List<FilterDefinition<CommentReaction>>
            {
                filterBuilder.Eq(r => r.CommentId, commentId),
                filterBuilder.Eq(r => r.IsDeleted, false)
            };
            if (!string.IsNullOrEmpty(lastSeenId))
            {
                filters.Add(filterBuilder.Gt(r => r.UserId, lastSeenId));
            }
            var filter = filterBuilder.And(filters);
            return await _collection.Find(filter)
                .SortBy(r => r.Id)
                .Limit(take)
                .Project(r => r.UserId)
                .ToListAsync();
        }

        public async Task<bool> IsCommentReactedByUserAsync(string commentId, string userId)
        {
            var filter = Builders<CommentReaction>.Filter.And(
                Builders<CommentReaction>.Filter.Eq(r => r.CommentId, commentId),
                Builders<CommentReaction>.Filter.Eq(r => r.UserId, userId),
                Builders<CommentReaction>.Filter.Eq(r => r.IsDeleted, false)
            );
            var count = await _collection.CountDocumentsAsync(filter);
            return count > 0;
        }
    }
}
