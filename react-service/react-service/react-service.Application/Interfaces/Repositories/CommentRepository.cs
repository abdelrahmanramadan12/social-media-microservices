using react_service.Domain.Entites;

namespace react_service.Application.Interfaces.Repositories
{
    public interface ICommentRepository
    {
        Task<bool> IsCommentDeleted(string commentId);
        Task<bool> DeleteComment(string commentId);
        Task<bool> AddComment(Comment comment);
        Task<Comment> GetCommentAsync(string commentId);
        Task<Comment> GetCommentByIdAsync(string commentId);

    }
}
