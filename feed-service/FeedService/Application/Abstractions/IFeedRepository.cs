using Application.DTOs;
using Domain.Entities;
using MongoDB.Bson;

namespace Application.Abstractions
{
    public interface IFeedRepository
    {
        Task<Response<Feed>> FindUserFeedAsync(string userId);
        Task IncrementCommentsCountAsync(string postId, int number);
        Task IncrementReactsCountAsync(string postId, int number);
        Task PushToFeedAsync(Post post, string userId);
        Task RemoveAuthorAsync(string userId);
        Task RemovePostAsync(string postId);
        Task RemoveUnfollowedPostsAsync(string followerId, string followingId);
        Task SetLikedAsync(string userId, string postId, bool liked);
        Task UpdateAuthorAsync(AuthorProfile authorProfile);
        Task UpdateContentAsync(Post post);
    }
}
