using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using Domain.ValueObjects;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly string _connectionString;
        private readonly string _dataBaseName;
        private readonly IMongoCollection<Post> _posts;
        private readonly FilterDefinition<Post> NotDeletedFilter = Builders<Post>.Filter.Eq(p => p.IsDeleted, false);
        public PostRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            _dataBaseName = configuration.GetSection("DatabaseName").Value!;
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_dataBaseName);
            _posts = database.GetCollection<Post>("Posts");
        }

        public async Task<Post> CreatePostAsync(Post post, bool HasMedia)
        {
            post.CreatedAt = DateTime.UtcNow;
            post.IsDeleted = false;
            post.NumberOfComments = 0;
            post.NumberOfLikes = 0;
            post.MediaList = [];

            if (HasMedia && post.MediaList.Count > 0)
            {
                foreach (var media in post.MediaList)
                    post.MediaList.Add(media);
            }

            await _posts.InsertOneAsync(post);

            return post;

        }

        public async Task<bool> DeletePostAsync(string postId, string postAuthorId)
        {
            var postIdFilter = Builders<Post>.Filter.Eq(p => p.Id, postId);
            var postAuthorFilter = Builders<Post>.Filter.Eq(p => p.AuthorId, postAuthorId);
            var filter = postIdFilter & postAuthorFilter & NotDeletedFilter;

            var update = Builders<Post>.Update
                .Set(p => p.IsDeleted, true)
                .Set(p => p.DeletedAt, DateTime.UtcNow);
            var result = await _posts.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

        public async Task<Post> GetPostAsync(string postId)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.Id, postId) & NotDeletedFilter;
            var post = await _posts.FindAsync(filter);

            return await post.FirstOrDefaultAsync();
        }

        public async Task<List<Post>> GetUserPostsAsync(string userId, int pageSize, string? cursorPostId)
        {

            var filter = Builders<Post>.Filter.Eq(p => p.AuthorId, userId) & NotDeletedFilter;

            if (string.IsNullOrEmpty(cursorPostId))
            {
                filter &= Builders<Post>.Filter.Lt(p => p.CreatedAt, DateTime.Now);
                var lastPost = await _posts.Find(filter).FirstOrDefaultAsync();
                if (lastPost == null) return [];
            }else
            {
                var lastPostFilter = Builders<Post>.Filter.Eq(p => p.Id, cursorPostId);
                var lastPost = await (await _posts.FindAsync(lastPostFilter)).FirstOrDefaultAsync();
                filter &= Builders<Post>.Filter.Lt(p => p.CreatedAt, lastPost.CreatedAt);
                if (lastPost == null) return [];
            }

            var posts = await _posts.Find(filter)
                                    .SortByDescending(p => p.CreatedAt)
                                    .Limit(pageSize)
                                    .ToListAsync();
            return posts;

        }

        public async Task<Post?> UpdatePostAsync(string postId, Post newPost, bool HasMedia)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.Id, postId) & NotDeletedFilter;
            var update = Builders<Post>.Update
                .Set(p => p.Content, newPost.Content)
                .Set(p => p.Privacy, newPost.Privacy)
                .Set(p => p.UpdatedAt, DateTime.UtcNow);

            if (HasMedia)
                update.Set(p => p.MediaList, newPost.MediaList);

            var result = await _posts.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0
                ? await GetPostAsync(postId)
                : null;
        }

        public async Task<bool> UpdateLikesCounter(CounterManager counterManager, string postId)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.Id, postId) & NotDeletedFilter;
            var update = Builders<Post>.Update;
            UpdateDefinition<Post> updateDefinition;

            var post = await _posts.Find(filter).FirstOrDefaultAsync();
            if (post == null) return false;

            if (post.NumberOfLikes == 0 && counterManager == CounterManager.DECREMENT) return false;

            if (counterManager == CounterManager.INCREMENT)
                updateDefinition = update.Set(p => p.NumberOfLikes, post.NumberOfLikes + 1);
            else
                updateDefinition = update.Set(p => p.NumberOfLikes, post.NumberOfLikes - 1);

            var result = await _posts.UpdateOneAsync(filter, updateDefinition);

            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateCommentsCounter(CounterManager counterManager, string postId)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.Id, postId) & NotDeletedFilter;
            var update = Builders<Post>.Update;
            UpdateDefinition<Post> updateDefinition;

            var post = await _posts.Find(filter).FirstOrDefaultAsync();

            if (post == null) return false;

            if (post.NumberOfComments == 0 && counterManager == CounterManager.DECREMENT) return false;

            if (counterManager == CounterManager.INCREMENT)
                updateDefinition = update.Set(p => p.NumberOfComments, post.NumberOfComments + 1);
            else
                updateDefinition = update.Set(p => p.NumberOfComments, post.NumberOfComments - 1);

            var result = await _posts.UpdateOneAsync(filter, updateDefinition);

            return result.ModifiedCount > 0;
        }
    }
}
