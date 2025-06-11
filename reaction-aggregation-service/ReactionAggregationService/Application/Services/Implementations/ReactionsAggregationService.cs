using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.Profile;
using Application.DTOs.Reactions;
using Application.Services.Interfaces;

namespace Application.Services.Implementations
{
    public class ReactionsAggregationService : IReactionsAggregationService
    {
        private readonly IReactionServiceClient _reactionServiceClient;
        private readonly IProfileServiceClient _profileServiceClient;

        public ReactionsAggregationService(IReactionServiceClient reactionServiceClient, IProfileServiceClient profileServiceClient)
        {
            _reactionServiceClient = reactionServiceClient ?? throw new ArgumentNullException(nameof(reactionServiceClient));
            _profileServiceClient = profileServiceClient ?? throw new ArgumentNullException(nameof(profileServiceClient));
        }
        public async Task<PaginationResponseWrapper<List<SimpleUserProfile>>> GetReactionsOfPostAsync(GetReactsOfPostRequest request)
        {
            if (request == null)
            {
                return new PaginationResponseWrapper<List<SimpleUserProfile>>
                {
                    Errors = new List<string> { "Request cannot be null." },
                    ErrorType = ErrorType.BadRequest,
                    Message = "Invalid request.",
                    HasMore = false
                };
            }

            if (string.IsNullOrWhiteSpace(request.PostId))
            {
                return new PaginationResponseWrapper<List<SimpleUserProfile>>
                {
                    Errors = new List<string> { "Post ID is required." },
                    Message = "Invalid request.",
                    HasMore = false
                };
            }

            try
            {
                // Get reactions for the post
                var reactions = await _reactionServiceClient.GetReactsOfPostAsync(request);

                if (!reactions.Success || reactions.Data == null)
                {
                    return new PaginationResponseWrapper<List<SimpleUserProfile>>
                    {
                        Errors = reactions.Errors ?? new List<string> { "Failed to retrieve reactions." },
                        Message = reactions.Message ?? "Failed to retrieve reactions.",
                        HasMore = false
                    };
                }

                if (!reactions.Data.Any())
                {
                    return new PaginationResponseWrapper<List<SimpleUserProfile>>
                    {
                        Data = new List<SimpleUserProfile>(),
                        Message = "No reactions found for this post.",
                        HasMore = false
                    };
                }
                var userProfileRequest = new GetUsersProfileByIdsRequest { UserIds = new List<string> { reactions.Data } };
                var userProfiles = await _profileServiceClient.GetUsersByIdsAsync(userProfileRequest);

                if (!userProfiles.Success)
                {
                    return new PaginationResponseWrapper<List<SimpleUserProfile>>
                    {
                        Errors = userProfiles.Errors ?? new List<string> { "Failed to retrieve user profiles." },
                        Message = userProfiles.Message ?? "Failed to retrieve user profiles.",
                        HasMore = false
                    };
                }

                return new PaginationResponseWrapper<List<SimpleUserProfile>>
                {
                    Data = userProfiles.Data ?? new List<SimpleUserProfile>(),
                    HasMore = reactions.HasMore,
                    Next = reactions.Next,
                    Message = string.IsNullOrWhiteSpace(reactions.Message) ? "Successfully retrieved reactions and user profiles." : reactions.Message,
                    Errors = reactions.Errors
                };
            }
            catch (Exception ex)
            {
                return new PaginationResponseWrapper<List<SimpleUserProfile>>
                {
                    Errors = new List<string> { ex.Message },
                    Message = "An unexpected error occurred while processing the request.",
                    HasMore = false
                };
            }
        }
    }
}
