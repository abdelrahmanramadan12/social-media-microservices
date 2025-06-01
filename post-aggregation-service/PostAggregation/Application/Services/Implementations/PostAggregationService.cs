using Application.DTOs;
using Application.DTOs.Follow;
using Application.DTOs.Post;
using Application.DTOs.Profile;
using Application.DTOs.Reaction;
using Application.Services.Interfaces;

namespace Application.Services.Implementations
{
    public class PostAggregationService : IPostAggregationService
    {
        private readonly IpostServiceClient _postServiceClient;
        private readonly IReactionServiceClient _reactionServiceClient;
        private readonly IProfileServiceClient _profileServiceClient;
        private readonly IFollowServiceClient _followServiceClient;

        public PostAggregationService(IpostServiceClient postServiceClient, IReactionServiceClient reactionServiceClient, IProfileServiceClient profileServiceClient, IFollowServiceClient followServiceClient)
        {
            _postServiceClient = postServiceClient;
            _reactionServiceClient = reactionServiceClient;
            _profileServiceClient = profileServiceClient;
            _followServiceClient = followServiceClient;
        }

        public async Task<ServiceResponseDTO<PostAggregationDTO>> GetProfilePosts(string userId, string targetUser, string nextPostHashId)
        {
            var response = new ServiceResponseDTO<PostAggregationDTO>
            {
                Success = false,
                Errors = new List<string>()
            };

            // 1. Get the follow status
            var followStatusResult = await _followServiceClient.IsFollower(new IsFollowerRequest
            {
                userId = userId,
                targetUserId = targetUser
            });

            if (!followStatusResult.Success)
            {
                response.Errors.AddRange(followStatusResult.Errors ?? new List<string> { "Failed to check follow status." });
                return response;
            }

            bool isFollower = followStatusResult.Item;

            // 2. Get the profile posts
            var postsResult = await _postServiceClient.GetProfilePostListAsync(userId,targetUser, page);

            if (!postsResult.Success)
            {
                response.Errors.AddRange(postsResult.Errors ?? new List<string> { "Failed to get profile posts." });
                return response;
            }

            // 3. Start profile and reactions calls in parallel
            var profileTask = _profileServiceClient.GetProfileResponse(new SingleProfileRequest
            {
                UserId = targetUser
            });

            var postIds = postsResult.Posts.Select(p => p.PostId).ToList();

            var reactedTask = _reactionServiceClient.GetReactedPostsAsync(new FilteredReactedPostListRequest
            {
                UserId = userId,
                PostIds = postIds
            });

            await Task.WhenAll(profileTask, reactedTask);

            var profileResult = await profileTask;
            var reactedResult = await reactedTask;

            if (!profileResult.Success)
            {
                response.Errors.AddRange(profileResult.Errors ?? new List<string> { "Failed to get user profile." });
                return response;
            }

            var postAuthorProfile = profileResult.Item;
            var likedPostIds = reactedResult.Success && reactedResult.ReactedPosts != null
                ? reactedResult.ReactedPosts
                : new List<string>();

            // 4. Map to PostAggregationDTO
            response.ItemList = MapToPostAggregationDTOList(postsResult.Posts, likedPostIds, new List<PostAuthorProfile>() { postAuthorProfile });
            response.NextPostHashId = postsResult.NextPostHashId;
            response.Success = true;
            return response;
        }

        public Task<ServiceResponseDTO<PostAggregationDTO>> GetReactedPosts(string userId, string nextPostHashId)
        {

            return null!;
        }

        private List<PostAggregationDTO> MapToPostAggregationDTOList( List<Post> posts, List<string> likedPostIds, List<PostAuthorProfile> authorProfiles)
        {
            // Create a lookup dictionary for quick author retrieval
            var authorProfileDict = authorProfiles.ToDictionary(p => p.UserId, p => p);

            return posts.Select(post => new PostAggregationDTO
            {
                AuthorId = post.AuthorId,
                PostId = post.PostId,
                PostContent = post.PostContent,
                Privacy = post.Privacy,
                MediaUrls = post.MediaUrls,
                CreatedAt = post.CreatedAt,
                IsEdited = post.IsEdited,
                NumberOfLikes = post.NumberOfLikes,
                NumberOfComments = post.NumberOfComments,
                IsLiked = likedPostIds.Contains(post.PostId),
                PostAuthorProfile = authorProfileDict.TryGetValue(post.AuthorId, out var profile) ? profile : null
            }).ToList();
        }

    }
}
