using Application.DTOs;

namespace Application.IServices
{
    public interface IPostService
    {
        // GetPost 
        public Task<ServiceResponse<PostResponseDTO>> GetPostByIdAsync(string postId);
        public Task<ServiceResponse<PostResponseDTO>> GetProfilePostListAsync(string userId, string targetUserId, int pageSize, string cursorPostId);
        public Task<ServiceResponse<PostResponseDTO>> GetPostListAsync(string userId, List<string> PostIds);
        public Task<ServiceResponse<PostResponseDTO>> AddPostAsync(string userId, PostInputDTO postInputDto);
        public Task<ServiceResponse<PostResponseDTO>> UpdatePostAsync(string userId, PostInputDTO postInputDto);
        public Task<ServiceResponse<string>> DeletePostAsync(string userId, string postId);


    }
}
