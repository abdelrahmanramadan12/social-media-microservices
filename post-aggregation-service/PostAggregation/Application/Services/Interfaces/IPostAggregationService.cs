using Application.DTOs;
using Application.DTOs.Aggregation;
using Application.DTOs.Post;

namespace Application.Services.Interfaces
{
    public interface IPostAggregationService
    {
        Task<PaginationResponseWrapper<List<PostAggregationResponse>>> GetProfilePosts(ProfilePostsRequest request);
        Task<PaginationResponseWrapper<List<PostAggregationResponse>>> GetReactedPosts(ReactedPostsRequest request);
        Task<ResponseWrapper<PostAggregationResponse>> GetSinglePost(GetSinglePostRequest request);
    }
}
