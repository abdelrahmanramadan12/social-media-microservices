using Domain.DTOs;
using Service.DTOs.Requests;
using Service.DTOs.Responses;

namespace Service.Interfaces.CommentServices
{
    public interface ICommentService
    {
        Task<ResponseWrapper<CommentResponse>> CreateCommentAsync(CreateCommentRequest dto);

        Task<ResponseWrapper<PagedCommentsResponse>> ListCommentsAsync(GetPagedCommentRequest request);

        Task<ResponseWrapper<CommentResponse>> UpdateCommentAsync(EditCommentRequest dto);

        Task<ResponseWrapper<bool>> DeleteCommentAsync(string CommentId, string userId);
    }
}
