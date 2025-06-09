using Application.Abstractions;
using Application.Events;
using Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Services
{
    public class FeedCommandService : IFeedCommandService
    {
        private readonly IFeedRepository _feedRepository;
        private readonly IFollowServiceClient _followServiceClient;
        private readonly IProfileServiceClient _profileServiceClient;

        public FeedCommandService(
            IFeedRepository feedRepository,
            IFollowServiceClient followServiceClient,
            IProfileServiceClient profileServiceClient)
        {
            _feedRepository = feedRepository;
            _followServiceClient = followServiceClient;
            _profileServiceClient = profileServiceClient;
        }

        public async Task PushToFeedsAsync(PostEvent postEvent)
        {
            var followersRes = await _followServiceClient.ListFollowersAsync(postEvent.AuthorId);

            if (followersRes.Success && followersRes.Value.Follows.Count > 0)
            {
                var profileRes = await _profileServiceClient.GetProfileAsync(postEvent.AuthorId);

                if (profileRes.Success)
                {
                    // initialize author profile info
                    AuthorProfile authorProfile = new AuthorProfile
                    {
                        Id = postEvent.AuthorId,
                        UserName = profileRes.Value.UserName,
                        DisplayName = profileRes.Value.DisplayName,
                        ProfilePictureUrl = profileRes.Value.ProfilePictureUrl
                    };

                    // initialize post
                    Post post = new Post
                    {
                        AuthorProfile = authorProfile,
                        PostId = postEvent.Id,
                        Content = postEvent.Content,
                        MediaList = postEvent.MediaList,
                        CreatedAt = postEvent.Timestamp,
                        IsEdited = postEvent.IsEdited,
                        Privacy = postEvent.Privacy,
                        IsLiked = false,
                        CommentsCount = 0,
                        ReactsCount = 0
                    };

                    foreach (var follow in followersRes.Value.Follows)
                    {
                        await _feedRepository.PushToFeedAsync(post, follow);
                    }
                }
            }
        }

        public async Task UpdateInFeedsAsync(PostEvent postEvent)
        {
            // initialize post
            Post post = new Post
            {
                PostId = postEvent.Id,
                Content = postEvent.Content,
                MediaList = postEvent.MediaList,
                IsEdited = postEvent.IsEdited,
                Privacy = postEvent.Privacy,
            };

            await _feedRepository.UpdateContentAsync(post);
        }

        public async Task RemoveFromFeedsAsync(PostEvent postEvent)
        {
            await _feedRepository.RemovePostAsync(postEvent.Id);
        }

        public async Task UpdateAuthorAsync(ProfileEvent profileEvent)
        {
            // initialize author profile info
            AuthorProfile authorProfile = new AuthorProfile
            {
                Id = profileEvent.UserId,
                UserName = profileEvent.UserName,
                DisplayName = profileEvent.DisplayName,
                ProfilePictureUrl = profileEvent.ProfilePictureUrl
            };

            await _feedRepository.UpdateAuthorAsync(authorProfile);
        }

        public async Task RemoveAuthorAsync(ProfileEvent profileEvent)
        {
            await _feedRepository.RemoveAuthorAsync(profileEvent.UserId);
        }

        public async Task IncrementCommentsCountAsync(CommentEvent commentEvent)
        {
            await _feedRepository.IncrementCommentsCountAsync(commentEvent.PostId, 1);
        }

        public async Task DecrementCommentsCountAsync(CommentEvent commentEvent)
        {
            await _feedRepository.IncrementCommentsCountAsync(commentEvent.PostId, -1);
        }

        public async Task IncrementReactsCountAsync(ReactEvent reactEvent)
        {
            await _feedRepository.IncrementReactsCountAsync(reactEvent.PostId, 1);
            await _feedRepository.SetLikedAsync(reactEvent.UserId, reactEvent.PostId, true);
        }

        public async Task DecrementReactsCountAsync(ReactEvent reactEvent)
        {
            await _feedRepository.IncrementReactsCountAsync(reactEvent.PostId, -1);
            await _feedRepository.SetLikedAsync(reactEvent.UserId, reactEvent.PostId, false);
        }

        public async Task RemoveUnfollowedPostsAsync(FollowEvent followEvent)
        {
            await _feedRepository.RemoveUnfollowedPostsAsync(followEvent.FollowerId, followEvent.FollowingId);
        }
    }
}
