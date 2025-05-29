using Application.DTOs.Post;

namespace Application.Services.Interfaces
{
    public interface IpostServiceClient
    {
        public Task<ProfilePostsResponse> GetProfilePosts(ProfilePostsRequest request);
    }
}