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

        /// <summary>
        /// Increments the ReactCount for a comment by 1. Returns false if comment doesn't exist or is deleted.
        /// </summary>
        Task<bool> IncrementReactionCountAsync(string commentId);

        /// <summary>
        /// Decrements the ReactCount for a comment by 1. Will not go below zero. Returns false if comment doesn't exist or is deleted.
        /// </summary>
        Task<bool> DecrementReactionCountAsync(string commentId);
    }
}
