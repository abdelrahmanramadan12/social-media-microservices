using Domain.Entities;

namespace Domain.IRepository
{
    public interface IPostRepository
    {
        // Create 
        Task<Post> CreatePostAsync(Post post, bool HasMedia);

        // Read
        Task<Post> GetPostAsync(string postId);
        Task<List<Post>> GetPostList(string userId, List<string> postIds);

        // Read List
        Task<List<Post>> GetUserPostsAsync(string userId, int pageSize, string? cursorPostId);

        // Update
        Task<Post> UpdatePostAsync(string postId, Post newPost, bool HasMedia);

        // Delete
        Task<bool> DeletePostAsync(string postId, string PostAuthorId);
    }
}
