using react_service.Application.DTO;
using react_service.Application.DTO.Reaction.Request.Comment;

namespace react_service.Application.Interfaces.Services
{
    public interface IReactionCommentService
    {
        Task<ResponseWrapper<bool>> DeleteReactionAsync(string commentId, string userId);
        Task<ResponseWrapper<bool>> AddReactionAsync(CreateCommentReactionRequest reaction, string userId);
        Task<ResponseWrapper<bool>> DeleteReactionsByCommentId(string commentId);
        Task<ResponseWrapper<List<string>>> FilterCommentsReactedByUserAsync(List<string> commentIds, string userId);
        Task<PaginationResponseWrapper<List<string>>> GetCommentsReactedByUserAsync(string userId, string? nextReactIdHash);
        Task<ResponseWrapper<List<string>>> GetUserIdsReactedToCommentAsync(string commentId);
        Task<PaginationResponseWrapper<List<string>>> GetUserIdsReactedToCommentAsync(string commentId, string next, int take);
        Task<ResponseWrapper<bool>> IsCommentReactedByUserAsync(string commentId, string userId);
    }
}
