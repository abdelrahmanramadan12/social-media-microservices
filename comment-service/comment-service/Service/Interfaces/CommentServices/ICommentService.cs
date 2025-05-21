using Domain.DTOs;

namespace Service.Interfaces.CommentServices
{
    public interface ICommentService
    {
        Task<CommentResponseDto> CreateCommentAsync(CreateCommentRequestDto dto);

        Task<PagedCommentsDto> ListCommentsAsync(string postId, string? nextCommentIdHash = null);

        Task<CommentResponseDto?> UpdateCommentAsync(EditCommentRequestDto dto);

        Task<bool> DeleteCommentAsync(string CommentId, string userId);
    }
}
