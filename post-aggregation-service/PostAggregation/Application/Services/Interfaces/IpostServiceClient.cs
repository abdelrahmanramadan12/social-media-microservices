using Application.DTOs;
using Application.DTOs.Post;

namespace Application.Services.Interfaces
{
    public interface IpostServiceClient
    {
        Task<ServiceResponse<PostResponseDTO>> GetPostByIdAsync(string postId);
        Task<ServiceResponse<PostResponseDTO>> GetProfilePostListAsync(string userId, string profileUserId, int pageSize, string nextCursor);
        Task<ServiceResponse<PostResponseDTO>> GetPostListAsync(string userId, List<string> postIds);
    }
}