using Domain.Entities;
using MongoDB.Bson;

namespace Application.Abstractions
{
    public interface IFeedRepository
    {
        Task<Feed> GetFeedAsync(ObjectId feedId);
        Task IncrementCommentsCountAsync(ObjectId postId, int number);
        Task IncrementReactsCountAsync(ObjectId postId, int number);
        Task PushToFeedAsync(Post post, string userId);
        Task RemoveAuthorAsync(string userId);
        Task RemovePostAsync(ObjectId postId);
        Task RemoveUnfollowedPostsAsync(string followerId, string followingId);
        Task SetLikedAsync(string userId, ObjectId postId, bool liked);
        Task UpdateAuthorAsync(AuthorProfile authorProfile);
        Task UpdateContentAsync(Post post);
    }
}
