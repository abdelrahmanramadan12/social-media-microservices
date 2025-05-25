using Application.DTOs;
using Application.Services;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IHelperService
    {
        public Task<ServiceResponse<Post>> UpdatePostMedia(PostInputDTO postInputDto, Post postToUpdate);
        public Task<ValidationResult> CheckPostAccess(string userId, Post post);
        public Task<MediaUploadResponse> AssignMediaToPostInput(PostInputDTO postInputDTO);
        public Task<MappingResult<PostResponseDTO>> MapPostToPostResponseDto(Post post, string userId, bool checkIsLiked, bool assignProfile);
    }
}
