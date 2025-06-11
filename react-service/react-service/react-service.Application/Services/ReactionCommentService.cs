using AutoMapper;
using Microsoft.Extensions.Options;
using react_service.Application.DTO;
using react_service.Application.DTO.Reaction.Request.Comment;
using react_service.Application.Events;
using react_service.Application.Helpers;
using react_service.Application.Interfaces.Publishers;
using react_service.Application.Interfaces.Repositories;
using react_service.Application.Interfaces.Services;
using react_service.Application.Pagination;
using react_service.Domain.Entites;

namespace react_service.Application.Services
{
    public class ReactionCommentService : IReactionCommentService
    {
        private readonly ICommentReactionRepository reactionRepository;
        private readonly ICommentRepository commentRepository;
        private readonly IMapper mapper;
        private readonly IOptions<PaginationSettings> paginationSetting;
        private readonly IQueuePublisher<CommentReactionEvent> reactionPublisher;
        private readonly IQueuePublisher<ReactionEventNoti> reationNotiPublisher;

        public ReactionCommentService(ICommentReactionRepository reactionRepository, ICommentRepository commentRepository, IMapper mapper, IOptions<PaginationSettings> paginationSetting
            , IQueuePublisher<CommentReactionEvent> reactionPublisher , IQueuePublisher<ReactionEventNoti> reactionNotiPublisher)
        {
            this.reactionRepository = reactionRepository;
            this.commentRepository = commentRepository;
            this.mapper = mapper;
            this.paginationSetting = paginationSetting;
            this.reactionPublisher = reactionPublisher;
            this.reationNotiPublisher = reactionNotiPublisher;  
        }



        public async Task<ResponseWrapper<bool>> DeleteReactionAsync(string commentId, string userId)
        {
            var response = new ResponseWrapper<bool>();
            if (string.IsNullOrEmpty(commentId))
            {
                response.Errors.Add("Comment ID cannot be null or empty.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            if (string.IsNullOrEmpty(userId))
            {
                response.Errors.Add("User ID cannot be null or empty.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            var commentDeleted = await commentRepository.IsCommentDeleted(commentId);

            if (commentDeleted)
            {
                response.Errors.Add("Comment deleted or doesn't exist");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            var deleted = await reactionRepository.DeleteReactionAsync(commentId, userId);
            if (!deleted)
            {
                response.Errors.Add("Reaction not found.");
                response.ErrorType = ErrorType.NotFound;
                return response;
            }
            // Publish ReactionEvent for delete
            await reactionPublisher.PublishAsync(new CommentReactionEvent
            {
                CommentId = commentId,
                UserId = userId,
                EventType = ReactionEventType.Unlike
            });

            response.Message = "Reaction deleted successfully.";
            response.Data = true;
            return response;
        }

        public async Task<ResponseWrapper<bool>> AddReactionAsync(CreateCommentReactionRequest reaction, string userId)
        {
            var response = new ResponseWrapper<bool>();
            if (string.IsNullOrEmpty(reaction.CommentId))
            {
                response.Errors.Add("Comment ID cannot be null or empty.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            if (string.IsNullOrEmpty(userId))
            {
                response.Errors.Add("User ID cannot be null or empty.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            var commentDeleted = await commentRepository.IsCommentDeleted(reaction.CommentId);
            if (commentDeleted)
            {
                response.Errors.Add("Comment deleted or doesn't exist");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            var comment = await commentRepository.GetCommentAsync(reaction.CommentId);

            var reactionObj = mapper.Map<CommentReaction>(reaction);
            reactionObj.UserId = userId;
            await reactionRepository.AddReactionAsync(reactionObj);
            var commentObj  = await commentRepository.GetCommentByIdAsync(reaction.CommentId);

            // Publish ReactionEvent for add
            await reactionPublisher.PublishAsync(new CommentReactionEvent
            {
                CommentId = reaction.CommentId,
                UserId = userId,
                EventType = ReactionEventType.Like
            });
            // Publish ReactionEventNoti
            await reationNotiPublisher.PublishAsync(new ReactionEventNoti
            {

                AuthorEntityId = commentObj.AuthorId,
                Id = userId,
                ReactionEntityId = reactionObj.CommentId,
                User = new UserSkeleton
                {
                    Id = reactionObj.Id,
                    UserId = userId,
                    Seen = false,
                    CreatedAt = DateTime.UtcNow
                },
                Type = reactionObj.ReactionType,
                ReactedOn = ReactedEntity.Comment
            });

            response.Message = "Reaction added successfully.";
            response.Data = true;
            return response;
        }

        public async Task<ResponseWrapper<bool>> DeleteReactionsByCommentId(string commentId)
        {
            var response = new ResponseWrapper<bool>();
            if (string.IsNullOrEmpty(commentId))
            {
                response.Errors.Add("Comment ID cannot be null or empty.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            var commentDeleted = await commentRepository.IsCommentDeleted(commentId);
            if (commentDeleted)
            {
                response.Errors.Add("Comment deleted or doesn't exist");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            var deleted = await reactionRepository.DeleteAllCommentReactions(commentId);
            if (!deleted)
            {
                response.Errors.Add("No reactions found for the given comment ID.");
                response.ErrorType = ErrorType.NotFound;
                return response;
            }
            response.Message = "Reactions deleted successfully.";
            return response;
        }

        public async Task<ResponseWrapper<List<string>>> FilterCommentsReactedByUserAsync(List<string> commentIds, string userId)
        {
            var response = new ResponseWrapper<List<string>>();
            if (commentIds == null || commentIds.Count == 0)
            {
                response.Errors.Add("Comment IDs cannot be null or empty.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            if (string.IsNullOrEmpty(userId))
            {
                response.Errors.Add("User ID cannot be null or empty.");
                response.ErrorType = ErrorType.UnAuthorized;
                return response;
            }
            var Ids = await reactionRepository.FilterCommentsReactedByUserAsync(commentIds, userId);
            response.Data = Ids ?? new List<string>();
            response.Message = "Filtered comments reacted by user successfully.";
            return response;
        }

        public async Task<PaginationResponseWrapper<List<string>>> GetCommentsReactedByUserAsync(string userId, string nextReactIdHash)
        {
            var response = new PaginationResponseWrapper<List<string>>();
            if (string.IsNullOrEmpty(userId))
            {
                response.Errors.Add("User ID cannot be null or empty.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            string lastSeenId = string.IsNullOrWhiteSpace(nextReactIdHash) ? "" : PaginationHelper.DecodeCursor(nextReactIdHash!);
            var reactionList = (await reactionRepository.GetCommentsReactedByUserAsync(userId, lastSeenId)).ToList();
            bool hasMore = reactionList.Count > (paginationSetting.Value.DefaultPageSize - 1);
            var lastId = hasMore ? reactionList.Last().Id : null;
            response.Data = reactionList.Select(r => r.Id).ToList();
            response.HasMore = hasMore;
            response.Next = lastId != null ? PaginationHelper.GenerateCursor(lastId) : null;

            response.Message = "Comments reacted by user retrieved successfully.";
            return response;
        }

        public async Task<ResponseWrapper<List<string>>> GetUserIdsReactedToCommentAsync(string commentId)
        {
            var response = new ResponseWrapper<List<string>>();
            if (string.IsNullOrEmpty(commentId))
            {
                response.Errors.Add("Comment ID cannot be null or empty.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            var commentDeleted = await commentRepository.IsCommentDeleted(commentId);
            if (commentDeleted)
            {
                response.Errors.Add("Comment deleted or doesn't exist");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            var userIds = await reactionRepository.GetUserIdsReactedToCommentAsync(commentId);
            response.Data = userIds;
            response.Message = "User IDs retrieved successfully.";
            return response;
        }

        public async Task<PaginationResponseWrapper<List<string>>> GetUserIdsReactedToCommentAsync(string commentId, string next, int take)
        {
            var response = new PaginationResponseWrapper<List<string>>();
            if (string.IsNullOrEmpty(commentId))
            {
                response.Errors.Add("Comment ID cannot be null or empty.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            string lastSeenId = string.IsNullOrWhiteSpace(next) ? "" : PaginationHelper.DecodeCursor(next);
            var commentDeleted = await commentRepository.IsCommentDeleted(commentId);
            if (commentDeleted)
            {
                response.Errors.Add("Comment deleted or doesn't exist");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            var userIds = await reactionRepository.GetUserIdsReactedToCommentAsync(commentId, lastSeenId, take + 1);
            bool hasMore = userIds.Count > take;
            string nextCursor = hasMore ? PaginationHelper.GenerateCursor(userIds[take]) : null;
            if (hasMore) userIds = userIds.Take(take).ToList();
            response.Data = userIds;
            response.HasMore = hasMore;
            response.Next = nextCursor;
            response.Message = "User IDs retrieved successfully.";
            return response;
        }

        public async Task<ResponseWrapper<bool>> IsCommentReactedByUserAsync(string commentId, string userId)
        {
            var response = new ResponseWrapper<bool>();
            if (string.IsNullOrEmpty(commentId))
            {
                response.Errors.Add("Comment ID cannot be null or empty.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            if (string.IsNullOrEmpty(userId))
            {
                response.Errors.Add("User ID cannot be null or empty.");
                response.ErrorType = ErrorType.UnAuthorized;
                return response;
            }
            try
            {
                var commentDeleted = await commentRepository.IsCommentDeleted(commentId);
                if (commentDeleted)
                {
                    response.Errors.Add("Comment deleted or doesn't exist");
                    response.ErrorType = ErrorType.BadRequest;
                    return response;
                }
                var isReacted = await reactionRepository.IsCommentReactedByUserAsync(commentId, userId);
                response.Data = isReacted;
                response.Message = "Checked if comment is reacted by user successfully.";
            }
            catch (Exception ex)
            {
                response.Errors.Add($"An error occurred: {ex.Message}");
                response.ErrorType = ErrorType.InternalServerError;
                return response;
            }
            return response;
        }
    }
}
