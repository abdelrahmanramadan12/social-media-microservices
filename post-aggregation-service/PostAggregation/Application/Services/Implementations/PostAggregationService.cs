using Application.DTOs;
using Application.DTOs.Aggregation;
using Application.DTOs.Follow;
using Application.DTOs.Post;
using Application.DTOs.Profile;
using Application.DTOs.Reaction;
using Application.Services.Interfaces;

namespace Application.Services.Implementations
{
    public class PostAggregationService : IPostAggregationService
    {
        private readonly IPostServiceClient _postServiceClient;
        private readonly IReactionServiceClient _reactionServiceClient;
        private readonly IProfileServiceClient _profileServiceClient;
        private readonly IFollowServiceClient _followServiceClient;
        private const int MIN_POSTS_COUNT = 5;

        public PostAggregationService(
            IPostServiceClient postServiceClient,
            IReactionServiceClient reactionServiceClient,
            IProfileServiceClient profileServiceClient,
            IFollowServiceClient followServiceClient)
        {
            _postServiceClient = postServiceClient;
            _reactionServiceClient = reactionServiceClient;
            _profileServiceClient = profileServiceClient;
            _followServiceClient = followServiceClient;
        }

        // Helper Methods
        private List<PostResponseDTO> FilterPostsByPrivacy(List<PostResponseDTO> posts, string userId)
        {
            return posts.Where(p =>
                (p.Privacy == Privacy.Public || p.Privacy == Privacy.Private) ||
                (p.Privacy == Privacy.OnlyMe && p.AuthorId == userId)
            ).ToList();
        }

        private async Task<ResponseWrapper<List<PostResponseDTO>>> FilterPostsByFollowStatus(List<PostResponseDTO> posts, string userId)
        {
            var authorIds = posts.Select(p => p.AuthorId).Distinct().ToList();
            var followersResult = await _followServiceClient.FilterFollowers(new FilterFollowStatusRequest
            {
                UserId = userId,
                OtherIds = authorIds
            });

            if (!followersResult.Success)
            {
                return new ResponseWrapper<List<PostResponseDTO>>
                {
                    Errors = followersResult.Errors ?? new List<string> { "Failed to check follow status." },
                    ErrorType = ErrorType.InternalServerError,
                    Message = "Failed to check follow status."
                };
            }

            var followedAuthorIdsSet = new HashSet<string>(followersResult.Data);
            return new ResponseWrapper<List<PostResponseDTO>>
            {
                Data = posts.Where(p => followedAuthorIdsSet.Contains(p.AuthorId)).ToList()
            };
        }

        private async Task<ResponseWrapper<HashSet<string>>> FilterPostsByIsLiked(List<string> postIds, string userId)
        {
            var isLikedResult = await _reactionServiceClient.FilterPostsReactedByUserAsync(new FilterPostsReactedByUserRequest
            {
                UserId = userId,
                PostIds = postIds
            });

            if (!isLikedResult.Success)
            {
                return new ResponseWrapper<HashSet<string>>
                {
                    Errors = isLikedResult.Errors ?? new List<string> { "Failed to check like status." },
                    ErrorType = ErrorType.InternalServerError,
                    Message = "Failed to check like status."
                };
            }

            return new ResponseWrapper<HashSet<string>>
            {
                Data = isLikedResult.Data.ToHashSet()
            };
        }

        private async Task<ResponseWrapper<Dictionary<string, SimpleUserProfile>>> GetAuthorProfiles(List<string> authorIds)
        {
            var profilesResult = await _profileServiceClient.GetUsersByIdsAsync(new GetUsersProfileByIdsRequest
            {
                UserIds = authorIds
            });

            if (!profilesResult.Success)
            {
                return new ResponseWrapper<Dictionary<string, SimpleUserProfile>>
                {
                    Errors = profilesResult.Errors ?? new List<string> { "Failed to get author profiles." },
                    ErrorType = ErrorType.InternalServerError,
                    Message = "Failed to get author profiles."
                };
            }

            return new ResponseWrapper<Dictionary<string, SimpleUserProfile>>
            {
                Data = profilesResult.Data.ToDictionary(p => p.UserId)
            };
        }

        private List<PostAggregationResponse> MapToAggregatedPosts(List<PostResponseDTO> posts, Dictionary<string, SimpleUserProfile> profileDict, HashSet<string> liked)
        {
            return posts.Select(post => new PostAggregationResponse
            {
                AuthorId = post.AuthorId,
                PostId = post.PostId,
                PostContent = post.PostContent,
                Privacy = post.Privacy,
                Media = post.Media,
                CreatedAt = post.CreatedAt,
                IsEdited = post.IsEdited,
                NumberOfLikes = post.NumberOfLikes,
                NumberOfComments = post.NumberOfComments,
                IsLiked = liked.Contains(post.PostId),
                PostAuthorProfile = profileDict.GetValueOrDefault(post.AuthorId)
            }).ToList();
        }

        private async Task<ResponseWrapper<SimpleUserProfile>> GetUserProfile(string userId)
        {
            var profileResult = await _profileServiceClient.GetByUserIdMinAsync(userId);
            if (!profileResult.Success)
            {
                return new ResponseWrapper<SimpleUserProfile>
                {
                    Errors = profileResult.Errors ?? new List<string> { "Failed to get user profile." },
                    ErrorType = ErrorType.InternalServerError,
                    Message = "Failed to get user profile."
                };
            }
            return new ResponseWrapper<SimpleUserProfile>
            {
                Data = profileResult.Data
            };
        }

        private async Task<ResponseWrapper<bool>> IsPostLiked(string userId, string postId)
        {
            var isLikedResult = await _reactionServiceClient.IsPostLikedByUser(new IsPostLikedByUserRequest
            {
                UserId = userId,
                PostId = postId,
            });
            if (!isLikedResult.Success)
            {
                return new ResponseWrapper<bool>
                {
                    Errors = isLikedResult.Errors ?? new List<string> { "Failed to check if the post is liked." },
                    ErrorType = ErrorType.InternalServerError,
                    Message = "Failed to check if the post is liked."
                };
            }
            return isLikedResult;
        }
        
        // core methods
        public async Task<PaginationResponseWrapper<List<PostAggregationResponse>>> GetReactedPosts(ReactedPostsRequest request)
        {
            var response = new PaginationResponseWrapper<List<PostAggregationResponse>>();

            // Get the reacted posts Ids from the reaction service 
            var reactedPostsResult = await _reactionServiceClient.GetPostsReactedByUserAsync(new GetPostsReactedByUserRequest
            {
                UserId = request.UserId,
                Next = request.Next,
            });

            if (!reactedPostsResult.Success)
            {
                response.Errors.AddRange(reactedPostsResult.Errors ?? new List<string> { "Failed to get reacted posts." });
                response.ErrorType = ErrorType.InternalServerError;
                response.Message = "Failed to get reacted posts.";
                return response;
            }

            if (reactedPostsResult.Data == null || !reactedPostsResult.Data.Any())
            {
                return new PaginationResponseWrapper<List<PostAggregationResponse>>
                {
                    Data = new List<PostAggregationResponse>(),
                    Message = "No reacted posts found.",
                    Next = ""
                };
            }

            var reactedPostIds = reactedPostsResult.Data;
            List<PostResponseDTO> postList = new();
            bool hasMore = reactedPostsResult.HasMore;
            string nextCursor = reactedPostsResult.Next;
            var fetchedPostIds = new HashSet<string>(reactedPostIds);
                


                    if (!reactedPostsResult.Success)
                    {
                        response.Errors.AddRange(reactedPostsResult.Errors ?? new List<string> { "Failed to get more reacted posts." });
                        response.ErrorType = ErrorType.InternalServerError;
                        response.Message = "Failed to get more reacted posts.";
                        return response;
                    }

                    reactedPostIds = reactedPostsResult.Data;
                    hasMore = reactedPostsResult.HasMore;
                    nextCursor = reactedPostsResult.Next;
                

                var postsResult = await _postServiceClient.GetPostListAsync(request.UserId, reactedPostIds);

                if (!postsResult.Success)
                {
                    response.Errors.AddRange(postsResult.Errors ?? new List<string> { "Failed to load the posts!" });
                    response.ErrorType = ErrorType.InternalServerError;
                    response.Message = "Failed to load the posts!";
                    return response;
                }

                var newPosts = postsResult.Data ?? new List<PostResponseDTO>();
                newPosts = FilterPostsByPrivacy(newPosts, request.UserId);
                postList.AddRange(newPosts);
                fetchedPostIds.UnionWith(newPosts.Select(p => p.PostId));

                if (postList.Count < MIN_POSTS_COUNT && hasMore)
                {
                    reactedPostsResult = await _reactionServiceClient.GetPostsReactedByUserAsync(new GetPostsReactedByUserRequest
                    {
                        UserId = request.UserId,
                        Next = nextCursor
                    });

                    if (!reactedPostsResult.Success)
                    {
                        response.Errors.AddRange(reactedPostsResult.Errors ?? new List<string> { "Failed to get more reacted posts." });
                        response.ErrorType = ErrorType.InternalServerError;
                        response.Message = "Failed to get more reacted posts.";
                        return response;
                    }

                    reactedPostIds = reactedPostsResult.Data;
                    hasMore = reactedPostsResult.HasMore;
                    nextCursor = reactedPostsResult.Next;
                }
            

            if (!postList.Any())
            {
                return new PaginationResponseWrapper<List<PostAggregationResponse>>
                {
                    Data = new List<PostAggregationResponse>(),
                    Message = "No accessible posts found.",
                    Next = ""
                };
            }

            var followFilterResult = await FilterPostsByFollowStatus(postList, request.UserId);
            if (!followFilterResult.Success)
            {
                response.Errors.AddRange(followFilterResult.Errors);
                response.ErrorType = followFilterResult.ErrorType;
                response.Message = followFilterResult.Message;
                return response;
            }

            //postList = followFilterResult.Data;
            var authorIds = postList.Select(p => p.AuthorId).Distinct().ToList();
            var profilesResult = await GetAuthorProfiles(authorIds);

            if (!profilesResult.Success)
            {
                response.Errors.AddRange(profilesResult.Errors);
                response.ErrorType = profilesResult.ErrorType;
                response.Message = profilesResult.Message;
                return response;
            }

            var aggregatedPosts = MapToAggregatedPosts(postList, profilesResult.Data, postList.Select(p => p.PostId).ToHashSet());

            return new PaginationResponseWrapper<List<PostAggregationResponse>>
            {
                Data = aggregatedPosts,
                Message = "Reacted posts retrieved successfully.",
                Next = hasMore ? nextCursor : ""
            };
        }

        public async Task<PaginationResponseWrapper<List<PostAggregationResponse>>> GetProfilePosts(ProfilePostsRequest request)
        {
            var response = new PaginationResponseWrapper<List<PostAggregationResponse>>();

            var postsResult = await _postServiceClient.GetProfilePostListAsync(request.UserId, request.OtherId, request.Next);

            if (!postsResult.Success)
            {
                response.Errors.AddRange(postsResult.Errors ?? new List<string> { "Failed to get profile posts." });
                response.ErrorType = ErrorType.InternalServerError;
                response.Message = "Failed to get profile posts.";
                return response;
            }

            var postList = postsResult.Data ?? new List<PostResponseDTO>();
            bool hasMore = postsResult.HasMore;
            string nextCursor = postsResult.Next;

            if (!postList.Any())
            {
                return new PaginationResponseWrapper<List<PostAggregationResponse>>
                {
                    Data = new List<PostAggregationResponse>(),
                    Message = "No posts found.",
                    Next = ""
                };
            }

            postList = FilterPostsByPrivacy(postList, request.UserId);

            if (!postList.Any())
            {
                return new PaginationResponseWrapper<List<PostAggregationResponse>>
                {
                    Data = new List<PostAggregationResponse>(),
                    Message = "No accessible posts found.",
                    Next = ""
                };
            }

            var followFilterResult = await FilterPostsByFollowStatus(postList, request.UserId);
            if (!followFilterResult.Success)
            {
                response.Errors.AddRange(followFilterResult.Errors);
                response.ErrorType = followFilterResult.ErrorType;
                response.Message = followFilterResult.Message;
                return response;
            }

            var authorIds = postList.Select(p => p.AuthorId).Distinct().ToList();
            var profilesResult = await GetAuthorProfiles(authorIds);

            if (!profilesResult.Success)
            {
                response.Errors.AddRange(profilesResult.Errors);
                response.ErrorType = profilesResult.ErrorType;
                response.Message = profilesResult.Message;
                return response;
            }

            var likedPostsResult = await FilterPostsByIsLiked(postList.Select(p => p.PostId).ToList(), request.UserId);

            if (!likedPostsResult.Success)
            {
                response.Errors.AddRange(likedPostsResult.Errors);
                response.ErrorType = likedPostsResult.ErrorType;
                response.Message = likedPostsResult.Message;
                return response;
            }

            var aggregatedPosts = MapToAggregatedPosts(postList, profilesResult.Data, likedPostsResult.Data);

            return new PaginationResponseWrapper<List<PostAggregationResponse>>
            {
                Data = aggregatedPosts,
                Message = "Profile posts retrieved successfully.",
                Next = hasMore ? nextCursor : ""
            };
        }
        
        public async Task<ResponseWrapper<PostAggregationResponse>> GetSinglePost(GetSinglePostRequest request)
        {
            var response = new ResponseWrapper<PostAggregationResponse>();
            var postResult = await _postServiceClient.GetPostByIdAsync(request.PostId);
            if (!postResult.Success)
            {
                response.Errors.AddRange(postResult.Errors ?? new List<string> { "Failed to get the post." });
                response.ErrorType = response.ErrorType != ErrorType.None ? response.ErrorType : ErrorType.InternalServerError;
                response.Message = "Failed to get the post.";
                return response;
            }
            var post = postResult.Data;

            var profileResult = await GetUserProfile(post.AuthorId);
            if (!profileResult.Success)
            {
                response.Errors.AddRange(profileResult.Errors ?? new List<string> { "Failed to get the author profile." });
                response.ErrorType = profileResult.ErrorType != ErrorType.None ? profileResult.ErrorType : ErrorType.InternalServerError;
                response.Message = "Failed to get the author profile.";
                return response;
            }

            if (post.AuthorId != request.UserId)
            {
                switch (post.Privacy)
                {
                    case Privacy.Public:
                        break;
                    case Privacy.Private:
                        var followersResult = await _followServiceClient.FilterFollowers(new FilterFollowStatusRequest
                        {
                            UserId = request.UserId,
                            OtherIds = new List<string> { post.AuthorId }
                        });

                        if (!followersResult.Success)
                        {
                            response.Errors.AddRange(followersResult.Errors ?? new List<string> { "Failed to check follow status." });
                            response.ErrorType = ErrorType.InternalServerError;
                            response.Message = "Failed to check follow status.";
                            return response;
                        }

                        if (!followersResult.Data.Contains(post.AuthorId))
                        {
                            response.Errors.Add("You don't have access to this post.");
                            response.ErrorType = ErrorType.UnAuthorized;
                            response.Message = "You don't have access to this post.";
                            return response;
                        }
                        break;
                    case Privacy.OnlyMe:
                        // OnlyMe posts are only accessible to the author
                        response.Errors.Add("You don't have access to this post.");
                        response.ErrorType = ErrorType.UnAuthorized;
                        response.Message = "You don't have access to this post.";
                        return response;
                }
            }

            // Check if the post is liked by the user
            var isLikedResult = await IsPostLiked(request.UserId, request.PostId);
            if (!isLikedResult.Success)
            {
                response.Errors.AddRange(isLikedResult.Errors ?? new List<string> { "Failed to check if post is liked." });
                response.ErrorType = ErrorType.InternalServerError;
                response.Message = "Failed to check if post is liked.";
                return response;
            }

            // Create a PostAggregationResponse with the like status
            var aggregatedPost = new PostAggregationResponse
            {
                AuthorId = post.AuthorId,
                PostId = post.PostId,
                PostContent = post.PostContent,
                Privacy = post.Privacy,
                Media = post.Media,
                CreatedAt = post.CreatedAt,
                IsEdited = post.IsEdited,
                NumberOfLikes = post.NumberOfLikes,
                NumberOfComments = post.NumberOfComments,
                IsLiked = isLikedResult.Data,
                PostAuthorProfile = profileResult.Data
            };

            return new ResponseWrapper<PostAggregationResponse>
            {
                Data = aggregatedPost,
                Message = "Post retrieved successfully."
            };
        }
    }
}
