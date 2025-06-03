using Microsoft.Extensions.Options;
using MongoDB.Driver;
using react_service.Domain.Entites;
using react_service.Infrastructure.Mongodb;
using react_service.Application.Interfaces.Repositories;
using react_service.Application.Pagination;
using System.Xml.XPath;

namespace react_service.Infrastructure.Repositories
{
    public class ReactionPostRepository : IReactionPostRepository
    {
        private readonly IMongoCollection<ReactionPost> _collection;
        private readonly IMongoClient mongoClient;

        public IOptions<PaginationSettings> PaginationSetting { get; }

        public ReactionPostRepository(IOptions<MongoDbSettings> mongodbsettings, IMongoClient mongoClient, IOptions<PaginationSettings> paginationSetting)
        {
            var database = mongoClient.GetDatabase(mongodbsettings.Value.DatabaseName);
            _collection = database.GetCollection<ReactionPost>("ReactionPost");
            this.mongoClient = mongoClient;
            PaginationSetting = paginationSetting;
            // Ensure indexes are created once
            CreateIndexes();
        }

        private void CreateIndexes()
        {
            var indexKeys = Builders<ReactionPost>.IndexKeys
                .Ascending(r => r.UserId)
                .Ascending(r => r.PostId);

            var indexOptions = new CreateIndexOptions { Unique = true };

            var indexModel = new CreateIndexModel<ReactionPost>(indexKeys, indexOptions);

            _collection.Indexes.CreateOne(indexModel);
        }

        public async Task<List<ReactionPost>> GetReactsOfPostAsync(string postId, string nextReactIdHash)
        {

            var filterBuilder = Builders<ReactionPost>.Filter;
            var filters = new List<FilterDefinition<ReactionPost>>
            {
                filterBuilder.Eq(r => r.PostId, postId)
            };

            if (!string.IsNullOrEmpty(nextReactIdHash))
            {
                filters.Add(filterBuilder.Lt(r => r.Id, nextReactIdHash));
            }

            var filter = filterBuilder.And(filters);

            return await _collection
                .Find(filter)
                .SortByDescending(r => r.PostCreatedTime)
                .Limit(PaginationSetting.Value.DefaultPageSize)
                .ToListAsync();
        }
        // check i the user and post id are exist in database 
        public async Task<bool> CheckUserAndPostIdExist(string postId, string userId)
        {
            var filter = Builders<ReactionPost>.Filter.And(
                Builders<ReactionPost>.Filter.Eq(r => r.PostId, postId),
                Builders<ReactionPost>.Filter.Eq(r => r.UserId, userId)
            );
            var count = await _collection.CountDocumentsAsync(filter);
            return count > 0;
        }

        public async Task<List<ReactionPost>> GetPostsReactedByUserAsync(string userId, string nextReactIdHash)
        {
            var filterBuilder = Builders<ReactionPost>.Filter;
            var filters = new List<FilterDefinition<ReactionPost>>
            {
                filterBuilder.Eq(r => r.UserId, userId)
            };

            if (!string.IsNullOrEmpty(nextReactIdHash))
            {
                filters.Add(filterBuilder.Lt(r => r.Id, nextReactIdHash));
            }

            var filter = filterBuilder.And(filters);

            return await _collection
                .Find(filter)
                .SortByDescending(r => r.PostCreatedTime)
                .Limit(PaginationSetting.Value.DefaultPageSize)
                .ToListAsync();
        }

        public async Task<string> AddReactionAsync(ReactionPost reaction)
        {
            // var session = await mongoClient.StartSessionAsync(); 

            //session.StartTransaction();
            try
            {
                var reactionExist = await CheckUserAndPostIdExist(reaction.PostId, reaction.UserId);

                if (reactionExist)
                {
                    await DeleteReactionAsync(reaction.PostId, reaction.UserId);
                }

                await _collection.InsertOneAsync(reaction);

                //   await session.CommitTransactionAsync();

                return reaction.PostId;
            }
            catch (Exception ex)
            {
                // await session.AbortTransactionAsync();
                throw new Exception("Failed to create reaction within transaction", ex);
            }
        }

        public async Task<bool> DeleteReactionAsync(string postId, string userId)
        {
            var reationExist = await CheckUserAndPostIdExist(postId, userId);
            if (reationExist)
            {
                var res = await _collection.DeleteOneAsync(r => r.PostId == postId && r.UserId == userId);
                return res.DeletedCount > 0;
            }
            return false;

        }

        public async Task<bool> DeleteAllPostReactions(string postId)
        {
            var filter = Builders<ReactionPost>.Filter.Eq(r => r.PostId, postId);
            var result = await _collection.DeleteManyAsync(filter);
            return result.DeletedCount > 0;
        }

        public Task<List<string>> FilterPostsReactedByUserAsync(List<string> postIds, string userId)
        {
            var filter = Builders<ReactionPost>.Filter.And(
                Builders<ReactionPost>.Filter.In(r => r.PostId, postIds),
                Builders<ReactionPost>.Filter.Eq(r => r.UserId, userId)
            );
            return _collection.Find(filter)
                .Project(r => r.PostId)
                .ToListAsync();
        }

        public async Task<List<string>> GetUserIdsReactedToPostAsync(string postId)
        {
            var filter = Builders<ReactionPost>.Filter.Eq(r => r.PostId, postId);
            return await _collection.Find(filter)
                .Project(r => r.UserId)
                .ToListAsync();
        }

        public async Task<List<string>> GetUserIdsReactedToPostAsync(string postId, string lastSeenId, int take)
        {
            var filterBuilder = Builders<ReactionPost>.Filter;
            var filters = new List<FilterDefinition<ReactionPost>>
            {
                filterBuilder.Eq(r => r.PostId, postId)
            };
            if (!string.IsNullOrEmpty(lastSeenId))
            {
                filters.Add(filterBuilder.Gt(r => r.Id, lastSeenId));
            }
            var filter = filterBuilder.And(filters);
            return await _collection.Find(filter)
                .SortBy(r => r.Id)
                .Limit(take)
                .Project(r => r.UserId)
                .ToListAsync();
        }
    }
}
