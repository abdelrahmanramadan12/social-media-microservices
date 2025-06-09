using Application.DTOs;
using Application.DTOs.Reaction;

namespace Application.Services.Interfaces
{
    public interface IReactionServiceClient
    {
        Task<ResponseWrapper<FilteredPostsReactedByUserResponse>> FilterPostsReactedByUserAsync(FilterPostsReactedByUserRequest request);
        Task<ResponseWrapper<GetPostsReactedByUserResponse>> GetPostsReactedByUserAsync(GetPostsReactedByUserRequest request);
        Task<ResponseWrapper<GetReactsOfPostResponse>> GetReactsOfPostAsync(GetReactsOfPostRequest request);
        Task<ResponseWrapper<bool>> IsPostLikedByUser(IsPostLikedByUserRequest request);
    }
}
