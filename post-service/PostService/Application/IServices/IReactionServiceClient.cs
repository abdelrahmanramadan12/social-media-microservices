using Application.DTOs;

namespace Application.IServices
{
    public interface IReactionServiceClient
    {
        Task<ReactedPostListResponse> GetReactedPostsAsync(ReactedPostListRequest request);
    }
}
