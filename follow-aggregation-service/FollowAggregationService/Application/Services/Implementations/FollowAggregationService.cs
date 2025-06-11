using Application.DTOs;
using Application.DTOs.Aggregation;
using Application.DTOs.Follow;
using Application.DTOs.Profile;
using Application.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace Application.Services.Implementations
{
    public class FollowAggregationService : IFollowAggregationService
    {
        private readonly IFollowServiceClient _followServiceClient;
        private readonly IProfileServiceClient _profileServiceClient;

        public FollowAggregationService(IFollowServiceClient followServiceClient, IProfileServiceClient profileServiceClient)
        {
            _followServiceClient = followServiceClient ?? throw new ArgumentNullException(nameof(followServiceClient));
            _profileServiceClient = profileServiceClient ?? throw new ArgumentNullException(nameof(profileServiceClient));
        }

        public async Task<PaginationResponseWrapper<List<ProfileAggregation>>> GetFollowers(FollowListRequest request)
        {
            return await AggregateProfileRelationshipsAsync(
                request,
                _followServiceClient.GetFollowers,
                isListingFollowersContext: true,
                noResultsMessage: "No followers found",
                failedToFetchMessage: "Failed to fetch followers",
                successMessage: "Followers retrieved successfully"
            );
        }

        public async Task<PaginationResponseWrapper<List<ProfileAggregation>>> GetFollowing(FollowListRequest request)
        {
            return await AggregateProfileRelationshipsAsync(
                request,
                _followServiceClient.GetFollowing,
                isListingFollowersContext: false,
                noResultsMessage: "No following found",
                failedToFetchMessage: "Failed to fetch following list",
                successMessage: "Following list retrieved successfully"
            );
        }

        private async Task<PaginationResponseWrapper<List<ProfileAggregation>>> AggregateProfileRelationshipsAsync(
            FollowListRequest request,
            Func<ListFollowPageRequest, Task<PaginationResponseWrapper<List<string>>>> getInitialUserIdsFunc,
            bool isListingFollowersContext,
            string noResultsMessage,
            string failedToFetchMessage,
            string successMessage)
        {
            try
            {
                // Step 1: Get the initial list of user IDs (followers of OtherId or users followed by OtherId)
                var initialUserIdsResponse = await getInitialUserIdsFunc(new ListFollowPageRequest
                {
                    UserId = request.OtherId, // User whose list is being fetched
                    Next = request.Next
                });

                if (!initialUserIdsResponse.Success)
                {
                    return new PaginationResponseWrapper<List<ProfileAggregation>>
                    {
                        Errors = initialUserIdsResponse.Errors ?? new List<string> { failedToFetchMessage },
                        ErrorType = initialUserIdsResponse.ErrorType,
                        Message = failedToFetchMessage
                    };
                }

                var initialUserIds = initialUserIdsResponse.Data ?? new List<string>();
                if (!initialUserIds.Any())
                {
                    return new PaginationResponseWrapper<List<ProfileAggregation>>
                    {
                        Data = new List<ProfileAggregation>(),
                        HasMore = false,
                        Next = string.Empty,
                        Message = noResultsMessage
                    };
                }

                bool isSelf = string.Equals(request.UserId, request.OtherId, StringComparison.OrdinalIgnoreCase);

                // Step 2: Fetch profile details for these users
                var profilesTask = _profileServiceClient.GetUsersByIdsAsync(new GetUsersProfileByIdsRequest { UserIds = initialUserIds });

                // Step 3: Determine relationships relative to the viewing user (request.UserId)
                // viewerFollowsThemTask: checks which of initialUserIds are followed by request.UserId
                var viewerFollowsThemTask = _followServiceClient.FilterFollowers(new FilterFollowStatusRequest { UserId = request.UserId, OtherIds = initialUserIds });
                // themFollowViewerTask: checks which of initialUserIds follow request.UserId
                var themFollowViewerTask = _followServiceClient.FilterFollowing(new FilterFollowStatusRequest { UserId = request.UserId, OtherIds = initialUserIds });
                
                // Step 4: Await all parallel requests
                await Task.WhenAll(profilesTask, viewerFollowsThemTask, themFollowViewerTask);

                var profilesResponse = await profilesTask;
                if (!profilesResponse.Success || profilesResponse.Data == null)
                {
                    return new PaginationResponseWrapper<List<ProfileAggregation>>
                    {
                        Errors = profilesResponse?.Errors ?? new List<string> { "Failed to fetch user profiles" },
                        ErrorType = profilesResponse?.ErrorType ?? ErrorType.InternalServerError,
                        Message = "Failed to fetch user profiles"
                    };
                }

                var viewerFollowsThemMap = new Dictionary<string, bool>();
                var viewerFollowsThemResult = await viewerFollowsThemTask;
                if (viewerFollowsThemResult?.Success == true && viewerFollowsThemResult.Data != null)
                {
                    viewerFollowsThemMap = viewerFollowsThemResult.Data.ToDictionary(id => id, _ => true);
                }

                var themFollowViewerMap = new Dictionary<string, bool>();
                var themFollowViewerResult = await themFollowViewerTask;
                if (themFollowViewerResult?.Success == true && themFollowViewerResult.Data != null)
                {
                    themFollowViewerMap = themFollowViewerResult.Data.ToDictionary(id => id, _ => true);
                }
                
                // Step 5: Construct ProfileAggregation result
                var result = new List<ProfileAggregation>();
                foreach (var profile in profilesResponse.Data)
                {
                    var pa = new ProfileAggregation
                    {
                        UserId = profile.UserId,
                        DisplayName = profile.DisplayName,
                        UserName = profile.UserName,
                        ProfilePictureUrl = profile.ProfilePictureUrl
                    };

                    // Definitions:
                    // pa.IsFollowing: True if request.UserId (viewer) follows profile.UserId.
                    // pa.IsFollower: True if profile.UserId follows request.UserId (viewer).

                    // Maps:
                    // viewerFollowsThemMap: True if viewer follows profile.UserId.
                    // themFollowViewerMap: True if profile.UserId follows viewer.

                    if (isListingFollowersContext) // Context: GetFollowers (listing followers of OtherId)
                    {
                        // For profile P (a follower of OtherId):
                        // IsFollowing: Does viewer follow P?
                        pa.IsFollowing = viewerFollowsThemMap.ContainsKey(profile.UserId);
                        // IsFollower: Does P follow viewer?
                        // If isSelf (viewer is OtherId), then P is a follower of viewer, so P *does* follow viewer. This should be true.
                        // The 'isSelf ||' pattern ensures this, as themFollowViewerMap will contain P if isSelf.
                        pa.IsFollower = isSelf || themFollowViewerMap.ContainsKey(profile.UserId);
                    }
                    else // Context: GetFollowing (listing users OtherId is following)
                    {
                        // For profile P (someone OtherId follows):
                        // IsFollowing: Does viewer follow P?
                        // If isSelf (viewer is OtherId), then viewer *is* following P. This should be true.
                        // The 'isSelf ||' pattern ensures this, as viewerFollowsThemMap will contain P if isSelf.
                        pa.IsFollowing = isSelf || viewerFollowsThemMap.ContainsKey(profile.UserId);
                        // IsFollower: Does P follow viewer?
                        pa.IsFollower = themFollowViewerMap.ContainsKey(profile.UserId);
                    }
                    result.Add(pa);
                }

                return new PaginationResponseWrapper<List<ProfileAggregation>>
                {
                    Data = result,
                    HasMore = initialUserIdsResponse.HasMore,
                    Next = initialUserIdsResponse.Next,
                    Message = successMessage
                };
            }
            catch (Exception ex)
            {
                return new PaginationResponseWrapper<List<ProfileAggregation>>
                {
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError,
                    Message = "An error occurred while processing your request"
                };
            }
        }
    }
}
