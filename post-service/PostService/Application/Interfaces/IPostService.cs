using Application.DTOs;
using Application.DTOs.Responses;

namespace Application.IServices
{
    public interface IPostService
    {
        // GetPost 
        public Task<ResponseWrapper<PostResponseDTO>> GetPostByIdAsync(string postId);
        public Task<PaginationResponseWrapper<List<PostResponseDTO>>> GetProfilePostListAsync(string userId, string targetUserId, int pageSize, string cursorPostId);
        public Task<ResponseWrapper<List<PostResponseDTO>>> GetPostListAsync(string userId, List<string> PostIds);
        public Task<ResponseWrapper<PostResponseDTO>> AddPostAsync(string userId, PostInputDTO postInputDto);
        public Task<ResponseWrapper<PostResponseDTO>> UpdatePostAsync(string userId, PostInputDTO postInputDto);
        public Task<ResponseWrapper<string>> DeletePostAsync(string userId, string postId);


    }
}
