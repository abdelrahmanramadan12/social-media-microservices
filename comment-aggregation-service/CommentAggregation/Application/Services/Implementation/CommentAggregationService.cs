using Application.DTOs;
using Application.DTOs.Aggregation;
using Application.DTOs.Application.DTOs;
using Application.DTOs.Comment;
using Application.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implementation
{
    public class CommentAggregationService : ICommentAggregationService
    {
        private readonly ICommentServiceClient _commentServiceClient;
        private readonly IProfileServiceClient _profileServiceClient;
        private readonly IReactionServiceClient _reactionServiceClient;
        public CommentAggregationService(ICommentServiceClient commentServiceClient, IProfileServiceClient profileServiceClient, IReactionServiceClient reactionServiceClient)
        {
            this._commentServiceClient = commentServiceClient ?? throw new ArgumentNullException(nameof(commentServiceClient));
            this._profileServiceClient = profileServiceClient ?? throw new ArgumentNullException(nameof(profileServiceClient));
            this._reactionServiceClient = reactionServiceClient ?? throw new ArgumentNullException(nameof(reactionServiceClient));
        }

        public async Task<PaginationResponseWrapper<List<AggregatedComment>>> GetCommentList(GetPagedCommentRequest request)
        {
            var result = new PaginationResponseWrapper<List<AggregatedComment>>();
            if (request == null || string.IsNullOrEmpty(request.PostId))
            {
                result.Errors.Add("PostId is required.");
                result.ErrorType = ErrorType.BadRequest;
                return result;
            }
            if (string.IsNullOrEmpty(request.Next))
            {
                request.Next = string.Empty;
            }

            // Fetch comments for the post
            var commentsResponse = await _commentServiceClient.GetPagedCommentList(request);
            if (!commentsResponse.Success)
            {
                return new PaginationResponseWrapper<List<AggregatedComment>>
                {
                    Errors = commentsResponse.Errors,
                    ErrorType = commentsResponse.ErrorType,
                    Message = commentsResponse.Message
                };
            }
            var comments = commentsResponse.Data ?? new List<CommentResponse>();
            if (comments.Count == 0)
            {
                return new PaginationResponseWrapper<List<AggregatedComment>>
                {
                    Data = new List<AggregatedComment>(),
                    HasMore = commentsResponse.HasMore,
                    Next = commentsResponse.Next,
                    Message = "No comments found."
                };
            }

            var authorIds = comments.Select(c => c.AuthorId).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();
            var commentIds = comments.Select(c => c.CommentId).ToList();

            
            var profileTask = _profileServiceClient.GetUsersByIdsAsync(new DTOs.Profile.GetUsersProfileByIdsRequest { UserIds = authorIds });
            var reactionTask = string.IsNullOrEmpty(request.Next) && string.IsNullOrEmpty(request.PostId) ? Task.FromResult<ResponseWrapper<List<string>>>(null) :
                _reactionServiceClient.FilterCommentsReactedByUserAsync(new DTOs.Reaction.FilterCommentsReactedByUserRequest { CommentIds = commentIds, UserId = request.Next });

            await Task.WhenAll(profileTask, reactionTask);
            var profileResult = await profileTask;
            var reactionResult = await reactionTask;

            // Error handling for profileResult
            if (profileResult == null || !profileResult.Success)
            {
                return new PaginationResponseWrapper<List<AggregatedComment>>
                {
                    Errors = profileResult?.Errors ?? new List<string> { "Failed to fetch author profiles." },
                    ErrorType = profileResult?.ErrorType ?? ErrorType.InternalServerError,
                    Message = profileResult?.Message ?? "Failed to fetch author profiles."
                };
            }
            // Error handling for reactionResult
            if (reactionResult == null || !reactionResult.Success)
            {
                return new PaginationResponseWrapper<List<AggregatedComment>>
                {
                    Errors = reactionResult?.Errors ?? new List<string> { "Failed to fetch reaction data." },
                    ErrorType = reactionResult?.ErrorType ?? ErrorType.InternalServerError,
                    Message = reactionResult?.Message ?? "Failed to fetch reaction data."
                };
            }

            var profileDict = (profileResult.Data ?? new List<DTOs.Profile.SimpleUserProfile>()).ToDictionary(p => p.UserId, p => p);
            var likedCommentIds = new HashSet<string>(reactionResult?.Data ?? new List<string>());

            // Aggregate
            var aggregated = comments.Select(c => new AggregatedComment
            {
                CommentId = c.CommentId,
                PostId = c.PostId,
                AuthorId = c.AuthorId,
                CommentContent = c.CommentContent,
                MediaUrl = c.MediaUrl,
                CreatedAt = c.CreatedAt,
                IsEdited = c.IsEdited,
                ReactionsCount = c.ReactionsCount,
                IsLiked = likedCommentIds.Contains(c.CommentId),
                CommentAuthor = profileDict.TryGetValue(c.AuthorId, out var profile) ? profile : null
            }).ToList();

            return new PaginationResponseWrapper<List<AggregatedComment>>
            {
                Data = aggregated,
                HasMore = commentsResponse.HasMore,
                Next = commentsResponse.Next,
                Message = commentsResponse.Message
            };
        }
    }
}
