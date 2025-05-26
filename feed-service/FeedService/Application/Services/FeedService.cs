using Application.Abstractions;
using Application.DTOs;
using Domain.Entities;

namespace Application.Services
{
    internal class FeedService
    {
        private readonly IFeedRepository _feedRepository;
        private readonly IFollowServiceClient _followServiceClient;
        private readonly IProfileServiceClient _profileServiceClient;

        public FeedService(
            IFeedRepository feedRepository,
            IFollowServiceClient followServiceClient,
            IProfileServiceClient profileServiceClient)
        {
            _feedRepository = feedRepository;
            _followServiceClient = followServiceClient;
            _profileServiceClient = profileServiceClient;
        }

        public async Task PushToFeedsAsync(PostEventDTO postEvent)
        {
            var followers = await _followServiceClient.ListFollowersAsync(postEvent.AuthorId);

            if (followers != null && followers.Follows.Count > 0)
            {
                var profile = await _profileServiceClient.GetProfileAsync(postEvent.AuthorId);

                // initialize author profile info
                AuthorProfile authorProfile = new AuthorProfile
                {
                    Id = profile.Id,
                    UserName = profile.UserName,
                    DisplayName = profile.DisplayName,
                    ProfilePictureUrl = profile.ProfilePictureUrl
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

                foreach (var follow in followers.Follows)
                {
                    await _feedRepository.PushToFeedAsync(post, follow);
                }
            }
        }

        public async Task UpdateInFeedsAsync(PostEventDTO postEvent)
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

        public async Task RemoveFromFeedsAsync(PostEventDTO postEvent)
        {
            await _feedRepository.RemovePostAsync(postEvent.Id);
        }

        public async Task UpdateAuthorAsync(ProfileEventDTO profileEvent)
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

        public async Task RemoveAuthorAsync(ProfileEventDTO profileEvent)
        {
            await _feedRepository.RemoveAuthorAsync(profileEvent.UserId);
        }

        public async Task IncrementCommentsCountAsync(CommentEventDTO commentEvent)
        {
            await _feedRepository.IncrementCommentsCountAsync(commentEvent.PostId, 1);
        }

        public async Task DecrementCommentsCountAsync(CommentEventDTO commentEvent)
        {
            await _feedRepository.IncrementCommentsCountAsync(commentEvent.PostId, -1);
        }

        public async Task IncrementReactsCountAsync(ReactEventDTO reactEvent)
        {
            await _feedRepository.IncrementReactsCountAsync(reactEvent.PostId, 1);
            await _feedRepository.SetLikedAsync(reactEvent.UserId, reactEvent.PostId, true);
        }

        public async Task DecrementReactsCountAsync(ReactEventDTO reactEvent)
        {
            await _feedRepository.IncrementReactsCountAsync(reactEvent.PostId, -1);
            await _feedRepository.SetLikedAsync(reactEvent.UserId, reactEvent.PostId, false);
        }

        public async Task RemoveUnfollowedPostsAsync(FollowEventDTO followEvent)
        {
            await _feedRepository.RemoveUnfollowedPostsAsync(followEvent.FollowerId, followEvent.FollowingId);
        }
    }
}
