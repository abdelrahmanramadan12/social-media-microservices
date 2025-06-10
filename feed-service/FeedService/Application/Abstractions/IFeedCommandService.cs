using Application.Events;

namespace Application.Abstractions
{
    public interface IFeedCommandService
    {
        Task DecrementCommentsCountAsync(CommentEvent commentEvent);
        Task DecrementReactsCountAsync(ReactEvent reactEvent);
        Task IncrementCommentsCountAsync(CommentEvent commentEvent);
        Task IncrementReactsCountAsync(ReactEvent reactEvent);
        Task PushToFeedsAsync(PostEvent postEvent);
        Task RemoveAuthorAsync(ProfileEvent profileEvent);
        Task RemoveFromFeedsAsync(PostEvent postEvent);
        Task RemoveUnfollowedPostsAsync(FollowEvent followEvent);
        Task UpdateAuthorAsync(ProfileEvent profileEvent);
        Task UpdateInFeedsAsync(PostEvent postEvent);
    }
}