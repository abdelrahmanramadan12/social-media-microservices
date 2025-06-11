using Microsoft.Extensions.Options;
using MongoDB.Driver;
using react_service.Application.Interfaces.Repositories;
using react_service.Application.Pagination;
using react_service.Domain.Entites;
using react_service.Infrastructure.Mongodb;

namespace react_service.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IMongoCollection<Comment> _posts;

        public CommentRepository(IOptions<MongoDbSettings> mongodbsettings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("CommentsDB");
            _posts = database.GetCollection<Comment>("Comments");
        }

        public async Task<bool> IsCommentDeleted(string postId)
        {
            var post = await _posts.Find(p => p.CommentId == postId).FirstOrDefaultAsync();
            if (post == null)
                return true;
            return post.IsDeleted;
        }

        public async Task<bool> DeleteComment(string postId)
        {
            var post = await _posts.Find(p => p.CommentId == postId).FirstOrDefaultAsync();
            if (post == null || post.IsDeleted)
                return false;
            var update = Builders<Comment>.Update.Set(p => p.IsDeleted, true);
            var result = await _posts.UpdateOneAsync(p => p.CommentId == postId, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> AddComment(Comment post)
        {
            var existing = await _posts.Find(p => p.CommentId == post.CommentId).FirstOrDefaultAsync();
            if (existing == null)
            {
                await _posts.InsertOneAsync(post);
                return true;
            }
            if (existing.IsDeleted)
            {
                var update = Builders<Comment>.Update.Set(p => p.IsDeleted, false);
                var result = await _posts.UpdateOneAsync(p => p.CommentId == post.CommentId, update);
                return result.ModifiedCount > 0;
            }
            return false;
        }
        // get post  by id 

        public async Task<Comment> GetCommentAsync(string postId)
        {
            var post = await _posts.Find(p => p.CommentId == postId && !p.IsDeleted).FirstOrDefaultAsync();
            return post ?? throw new KeyNotFoundException("Comment not found or is deleted.");
        }

        public async Task<Comment> GetCommentByIdAsync(string commentId)
        {
            var comment  = await _posts.Find(p => p.CommentId == commentId && !p.IsDeleted).FirstOrDefaultAsync();
            return comment ?? throw new KeyNotFoundException("Comment not found or is deleted.");
        }
    }
}
