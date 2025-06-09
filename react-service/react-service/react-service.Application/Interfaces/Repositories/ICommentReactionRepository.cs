using react_service.Domain.Entites;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace react_service.Application.Interfaces.Repositories
{
    public interface ICommentReactionRepository
    {
        Task<List<string>> FilterCommentsReactedByUserAsync(List<string> commentIds, string userId);
        Task<List<CommentReaction>> GetReactsOfCommentAsync(string commentId, string nextReactIdHash);
        Task<List<CommentReaction>> GetCommentsReactedByUserAsync(string userId, string nextReactIdHash);
        Task<bool> DeleteReactionAsync(string commentId, string userId);
        Task<string> AddReactionAsync(CommentReaction reaction);
        Task<bool> DeleteAllCommentReactions(string commentId);
        Task<List<string>> GetUserIdsReactedToCommentAsync(string commentId);
        Task<List<string>> GetUserIdsReactedToCommentAsync(string commentId, string lastSeenId, int take);
        Task<bool> IsCommentReactedByUserAsync(string commentId, string userId);
    }
}
