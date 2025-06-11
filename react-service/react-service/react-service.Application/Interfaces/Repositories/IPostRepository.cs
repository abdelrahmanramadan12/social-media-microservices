using System.Threading.Tasks;
using react_service.Domain.Entites;

namespace react_service.Application.Interfaces.Repositories
{
    public interface IPostRepository
    {
        Task<bool> IsPostDeleted(string postId);
        Task<bool> DeletePost(string postId);
        Task<bool> AddPost(Post post);
        Task<Post> GetPostAsync(string postId);

    }
}