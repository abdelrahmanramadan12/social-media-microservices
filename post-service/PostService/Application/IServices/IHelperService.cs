using Application.DTOs;
using Application.Services;
using Domain.Entities;

namespace Application.IServices
{
    public interface IHelperService
    {
        public Task<ServiceResponse<Post>> UpdatePostMedia(PostInputDTO postInputDto, Post postToUpdate);
        public Task<ValidationResult> CheckPostAccess(string userId, Post post);
        public Task<MediaUploadResponse> AssignMediaToPostInput(PostInputDTO postInputDTO);
        public Task<MappingResult<PostResponseDTO>> MapPostToPostResponseDto(Post post, string userId, bool checkIsLiked, bool assignProfile);
        public Task<ReactedPostListResponse> GetReactedPostList(List<string> userPostIds, string userId);
        public Task<ProfilesResponse> GetProfilesResponse(List<string> userIds);
        public List<PostResponseDTO> AgregatePostResponseList(List<Post> posts, List<PostAuthorProfile> profiles, List<string> likedPosts);
    }
}
