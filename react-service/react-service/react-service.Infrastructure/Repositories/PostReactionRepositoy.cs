using Microsoft.Extensions.Options;
using MongoDB.Driver;
using react_service.Application.Interfaces.Repositories;
using react_service.Application.Pagination;
using react_service.Domain.Entites;
using react_service.Infrastructure.Mongodb;

namespace react_service.Infrastructure.Repositories
{
    public class PostReactionRepositoy : IPostReactionRepository
    {
        private readonly IMongoCollection<PostReaction> _collection;
        private readonly IMongoClient mongoClient;

        public IOptions<PaginationSettings> PaginationSetting { get; }

        public PostReactionRepositoy(IOptions<MongoDbSettings> mongodbsettings, IMongoClient mongoClient, IOptions<PaginationSettings> paginationSetting)
        {
            var database = mongoClient.GetDatabase(mongodbsettings.Value.ReactionsDatabaseName);
            _collection = database.GetCollection<PostReaction>("PostReaction");
            this.mongoClient = mongoClient;
            PaginationSetting = paginationSetting;
            CreateIndexes();
        }

        private void CreateIndexes()
        {
            var indexKeys = Builders<PostReaction>.IndexKeys
                .Ascending(r => r.UserId)
                .Ascending(r => r.PostId);

            var indexOptions = new CreateIndexOptions { Unique = true };

            var indexModel = new CreateIndexModel<PostReaction>(indexKeys, indexOptions);

            _collection.Indexes.CreateOne(indexModel);
        }

        public async Task<List<PostReaction>> GetReactsOfPostAsync(string postId, string nextReactIdHash)
        {

            var filterBuilder = Builders<PostReaction>.Filter;
            var filters = new List<FilterDefinition<PostReaction>>
            {
                filterBuilder.Eq(r => r.PostId, postId),
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
        // check i the user and post id are exist in database
        public async Task<bool> CheckUserAndPostIdExist(string postId, string userId)
        {
            var filter = Builders<PostReaction>.Filter.And(
                Builders<PostReaction>.Filter.Eq(r => r.PostId, postId),
                Builders<PostReaction>.Filter.Eq(r => r.UserId, userId),
                Builders<PostReaction>.Filter.Eq(r => r.IsDeleted, false)
            );
            var count = await _collection.CountDocumentsAsync(filter);
            return count > 0;
        }

        public async Task<List<PostReaction>> GetPostsReactedByUserAsync(string userId, string nextReactIdHash)
        {
            var filterBuilder = Builders<PostReaction>.Filter;
            var filters = new List<FilterDefinition<PostReaction>>
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
                .SortByDescending(r => r.PostId)
                .Limit(PaginationSetting.Value.DefaultPageSize)
                .ToListAsync();
        }
        public async Task<bool> HardDeleteReactionAsync(string postId, string userId)
        {
            var filter = Builders<PostReaction>.Filter.And(
                Builders<PostReaction>.Filter.Eq(r => r.PostId, postId),
                Builders<PostReaction>.Filter.Eq(r => r.UserId, userId)
            );

            var result = await _collection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }


        public async Task<string> AddReactionAsync(PostReaction reaction)
        {
            // var session = await mongoClient.StartSessionAsync(); 

            //session.StartTransaction();
            try
            {
                var reactionExist = await CheckUserAndPostIdExist(reaction.PostId, reaction.UserId);
                var reationType = reaction.ReactionType;
                if (reactionExist)
                {
                    var reactionObj = await GetReactionByIdAsync(reaction.PostId, reaction.UserId);

                    if (reactionObj.ReactionType == reaction.ReactionType)
                    {
                        await HardDeleteReactionAsync(reaction.PostId, reaction.UserId); // Hard delete the existing reaction    
                        return "Deleted";

                    }


                    await HardDeleteReactionAsync(reaction.PostId, reaction.UserId); // Hard delete the existing reaction    
                    reaction.IsDeleted = false;



                }
                await _collection.InsertOneAsync(reaction);

                return "Created";

                //   await session.CommitTransactionAsync();

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
                var filter = Builders<PostReaction>.Filter.And(
                    Builders<PostReaction>.Filter.Eq(r => r.PostId, postId),
                    Builders<PostReaction>.Filter.Eq(r => r.UserId, userId)
                );
                var update = Builders<PostReaction>.Update.Set(r => r.IsDeleted, true);
                var result = await _collection.UpdateOneAsync(filter, update);
                return result.ModifiedCount > 0;
            }
            return false;

        }

        public async Task<bool> DeleteAllPostReactions(string postId)
        {
            var filter = Builders<PostReaction>.Filter.Eq(r => r.PostId, postId);
            var update = Builders<PostReaction>.Update.Set(r => r.IsDeleted, true);
            var result = await _collection.UpdateManyAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public Task<List<string>> FilterPostsReactedByUserAsync(List<string> postIds, string userId)
        {
            var filter = Builders<PostReaction>.Filter.And(
                Builders<PostReaction>.Filter.In(r => r.PostId, postIds),
                Builders<PostReaction>.Filter.Eq(r => r.UserId, userId),
                Builders<PostReaction>.Filter.Eq(r => r.IsDeleted, false)
            );
            return _collection.Find(filter)
                .Project(r => r.PostId)
                .ToListAsync();
        }

        public async Task<List<string>> GetUserIdsReactedToPostAsync(string postId)
        {
            var filter = Builders<PostReaction>.Filter.And(
                Builders<PostReaction>.Filter.Eq(r => r.PostId, postId),
                Builders<PostReaction>.Filter.Eq(r => r.IsDeleted, false)
            );

            return await _collection.Find(filter)
                .Project(r => r.UserId)
                .ToListAsync();
        }

        public async Task<List<string>> GetUserIdsReactedToPostAsync(string postId, string lastSeenId, int take)
        {
            var filterBuilder = Builders<PostReaction>.Filter;
            var filters = new List<FilterDefinition<PostReaction>>
            {
                filterBuilder.Eq(r => r.PostId, postId),
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
        public async Task<PostReaction?> GetReactionByIdAsync(string postId, string userId)
        {
            var filterBuilder = Builders<PostReaction>.Filter;

            var filters = new List<FilterDefinition<PostReaction>>
            {
                filterBuilder.Eq(r => r.PostId, postId),
                filterBuilder.Eq(r => r.UserId, userId)
            };

            var filter = filterBuilder.And(filters);

            return await _collection.Find(filter).FirstOrDefaultAsync();
        }


        public async Task<bool> IsPostReactedByUserAsync(string postId, string userId)
        {
            var filter = Builders<PostReaction>.Filter.And(
                Builders<PostReaction>.Filter.Eq(r => r.PostId, postId),
                Builders<PostReaction>.Filter.Eq(r => r.UserId, userId),
                Builders<PostReaction>.Filter.Eq(r => r.IsDeleted, false)
            );
            var count = await _collection.CountDocumentsAsync(filter);
            return count > 0;
        }

        public async Task<bool> SoftDeletePostReaction(string postId)
        {
            var filter = Builders<PostReaction>.Filter.Eq(r => r.PostId, postId);
            var update = Builders<PostReaction>.Update.Set(r => r.IsDeleted, true);
            var result = await _collection.UpdateManyAsync(filter, update);
            return result.ModifiedCount > 0;
        }
    }
}
