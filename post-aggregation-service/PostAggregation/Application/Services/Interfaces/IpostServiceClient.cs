using Application.DTOs;
using Application.DTOs.Post;

namespace Application.Services.Interfaces
{
    public interface IPostServiceClient
    {
        Task<ResponseWrapper<PostResponseDTO>> GetPostByIdAsync(string postId);
        Task<PaginationResponseWrapper<List<PostResponseDTO>>> GetProfilePostListAsync(string userId, string profileUserId, string nextCursor);
        Task<ResponseWrapper<List<PostResponseDTO>>> GetPostListAsync(string userId, List<string> postIds);
    }
}