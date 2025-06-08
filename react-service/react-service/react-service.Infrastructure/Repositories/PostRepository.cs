using MongoDB.Driver;
using react_service.Domain.Entites;
using react_service.Application.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace react_service.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly IMongoCollection<Post> _posts;

        public PostRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDb");
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("PostsDB");
            _posts = database.GetCollection<Post>("Posts");
        }

        public async Task<bool> IsPostDeleted(string postId)
        {
            var post = await _posts.Find(p => p.PostId == postId).FirstOrDefaultAsync();
            return post != null && post.IsDeleted;
        }

        public async Task<bool> DeletePost(string postId)
        {
            var post = await _posts.Find(p => p.PostId == postId).FirstOrDefaultAsync();
            if (post == null || post.IsDeleted)
                return false;
            var update = Builders<Post>.Update.Set(p => p.IsDeleted, true);
            var result = await _posts.UpdateOneAsync(p => p.PostId == postId, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> AddPost(Post post)
        {
            var existing = await _posts.Find(p => p.PostId == post.PostId).FirstOrDefaultAsync();
            if (existing == null)
            {
                await _posts.InsertOneAsync(post);
                return true;
            }
            if (existing.IsDeleted)
            {
                var update = Builders<Post>.Update.Set(p => p.IsDeleted, false);
                var result = await _posts.UpdateOneAsync(p => p.PostId == post.PostId, update);
                return result.ModifiedCount > 0;
            }
            return false;
        }
    }
}