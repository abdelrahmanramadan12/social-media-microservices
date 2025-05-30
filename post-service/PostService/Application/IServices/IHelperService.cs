using Application.DTOs;
using Application.Services;
using Domain.Entities;

namespace Application.IServices
{
    public interface IHelperService
    {
        public Task<ServiceResponse<Post>> UpdatePostMedia(PostInputDTO postInputDto, Post postToUpdate);
        public Task<MediaUploadResponse> AssignMediaToPostInput(PostInputDTO postInputDTO);
        public MappingResult<PostResponseDTO> MapPostToPostResponseDto(Post post);
        public List<PostResponseDTO> AgregatePostResponseList(List<Post> posts);
    }
}
