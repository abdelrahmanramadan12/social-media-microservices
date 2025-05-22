using Domain.Entities;

namespace Domain.IRepository
{
    public interface IPostRepository
    {
        Task<bool> AddPostAsync(Post post);
        Task<bool> DeletePostAsync(string postId);
        Task<Post?> GetPostByIdAsync(string postId);
    }
}
