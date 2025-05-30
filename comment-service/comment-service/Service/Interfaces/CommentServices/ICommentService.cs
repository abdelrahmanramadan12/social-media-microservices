using Domain.DTOs;

namespace Service.Interfaces.CommentServices
{
    public interface ICommentService
    {
        Task<CommentDto> CreateCommentAsync(CreateCommentRequestDto dto);

        Task<PagedCommentsDto> ListCommentsAsync(string postId, string? nextCommentIdHash = null);

        Task<CommentResponseDto?> GetCommentAsync(string commentId);

        Task<CommentDto?> UpdateCommentAsync(EditCommentRequestDto dto);

        Task<bool> DeleteCommentAsync(string CommentId, string userId);
    }
}
