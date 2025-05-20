using Domain.Entities;

namespace Domain.IRepository
{
    public interface ICommentRepository
    {
        Task<Comment?> GetByIdAsync(string id);

        // Skip-based
        Task<IEnumerable<Comment>> GetByPostIdAsync(
            string postId, int skip = 0, int limit = 10);

        // Cursor-based
        Task<IEnumerable<Comment>> GetByPostIdCursorAsync(
            string postId, string? afterCommentId, int limit = 10);

        Task CreateAsync(Comment comment);
        Task UpdateAsync(Comment comment);
        Task DeleteAsync(string id);
        Task DeleteByPostIdAsync(string postId);
    }
}
