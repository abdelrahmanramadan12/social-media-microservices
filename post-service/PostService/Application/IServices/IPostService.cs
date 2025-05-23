using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IPostService
    {
        // GetPost 
        public Task<ServiceResponse<PostResponseDTO>> GetPostByIdAsync(string userId, string postId);
        public Task<ServiceResponse<PostResponseDTO>> GetProfilePostListAsync(string userId, string profileUserId, int pageSize, string cursorPostId);
        public Task<ServiceResponse<PostResponseDTO>> GetReactedPostListAsync(string userId, int pageSize, string cursorPostId);
        public Task<ServiceResponse<PostResponseDTO>> AddPostAsync(string userId, PostInputDTO postInputDto);
        public Task<ServiceResponse<PostResponseDTO>> UpdatePostAsync(string userId, PostInputDTO postInputDto);
        public Task<bool> DeletePostAsync(string userId, string postId, out string message);

    }
}
