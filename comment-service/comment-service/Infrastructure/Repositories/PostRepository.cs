using Domain.Entities;
using Domain.IRepository;
using MongoDB.Driver;

namespace Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly IMongoCollection<Post> _posts;

        public PostRepository(IMongoDatabase db)
        {
            _posts = db.GetCollection<Post>("posts");
        }

        public async Task<bool> AddPostAsync(Post post)
        {
            try
            {
                await _posts.InsertOneAsync(post);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeletePostAsync(string postId)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.PostId, postId);
            var result = await _posts.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task<Post?> GetPostByIdAsync(string postId)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.PostId, postId);
            return await _posts.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdatePostAsync(Post post)
        {
            var filter = Builders<Post>.Filter.Eq(p => p.PostId, post.PostId);
            var result = await _posts.ReplaceOneAsync(filter, post);
            return result.ModifiedCount > 0;
        }
    }
}
