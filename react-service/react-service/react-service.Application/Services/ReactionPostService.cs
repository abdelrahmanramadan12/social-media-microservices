using AutoMapper;
using Microsoft.Extensions.Options;
using react_service.Application.DTO;
using react_service.Application.DTO.Reaction.Request.Post;
using react_service.Application.Events;
using react_service.Application.Helpers;
using react_service.Application.Interfaces.Publishers;
using react_service.Application.Interfaces.Repositories;
using react_service.Application.Interfaces.Services;
using react_service.Application.Pagination;
using react_service.Domain.Entites;

namespace react_service.Application.Services
{
    public class ReactionPostService : IReactionPostService
    {
        // private readonly HttpClient _httpClient; // Remove or comment out this unused field
        private readonly IPostReactionRepository reactionRepository;
        private readonly IPostRepository postRepository;
        private readonly IMapper mapper;
        private readonly IOptions<PaginationSettings> paginationSetting;
        private readonly IQueuePublisher<ReactionEvent> reactionPublisher;

        public ReactionPostService(IPostReactionRepository reactionRepository, IPostRepository postRepository, IMapper mapper, IOptions<PaginationSettings> paginationSetting
            , IQueuePublisher<ReactionEvent> reactionPublisher)
        {
            this.reactionRepository = reactionRepository;
            this.postRepository = postRepository;
            this.mapper = mapper;
            this.paginationSetting = paginationSetting;
            this.reactionPublisher = reactionPublisher;
        }



        public async Task<ResponseWrapper<bool>> DeleteReactionAsync(string postId, string userId)
        {
            var response = new ResponseWrapper<bool>();
            if (string.IsNullOrEmpty(postId))
            {
                response.Errors.Add("Post ID cannot be null or empty.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            if (string.IsNullOrEmpty(userId))
            {
                response.Errors.Add("User ID cannot be null or empty.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            var postDeleted = await postRepository.IsPostDeleted(postId);

            if (postDeleted)
            {
                response.Errors.Add("Post deleted or doesn't exist");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            var deleted = await reactionRepository.DeleteReactionAsync(postId, userId);
            if (!deleted)
            {
                response.Errors.Add("Reaction not found.");
                response.ErrorType = ErrorType.NotFound;
                return response;
            }
            // Publish ReactionEvent for delete
            await reactionPublisher.PublishAsync(new ReactionEvent
            {
                PostId = postId,
                UserId = userId,
                EventType = ReactionEventType.Unlike
            });



            response.Message = "Reaction deleted successfully.";
            response.Data = true;
            return response;
        }

        public async Task<ResponseWrapper<bool>> AddReactionAsync(CreatePostReactionRequest reaction, string userId)
        {
            var response = new ResponseWrapper<bool>();
            if (string.IsNullOrEmpty(reaction.PostId))
            {
                response.Errors.Add("Post ID cannot be null or empty.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            if (string.IsNullOrEmpty(userId))
            {
                response.Errors.Add("User ID cannot be null or empty.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            var postDeleted = await postRepository.IsPostDeleted(reaction.PostId);
            if (postDeleted)
            {
                response.Errors.Add("Post deleted or doesn't exist");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            var post = await postRepository.GetPostAsync(reaction.PostId);

            var reactionObj = mapper.Map<PostReaction>(reaction);
            reactionObj.UserId = userId;
            var res = await reactionRepository.AddReactionAsync(reactionObj);
            // Publish ReactionEvent for add
            await reactionPublisher.PublishAsync(new ReactionEvent
            {
                PostId = reaction.PostId,
                UserId = userId,
                EventType = ReactionEventType.Like
            });

            response.Data = true;
            return response;
        }

        public async Task<ResponseWrapper<bool>> DeleteReactionsByPostId(string postId)
        {
            var response = new ResponseWrapper<bool>();
            if (string.IsNullOrEmpty(postId))
            {
                response.Errors.Add("Post ID cannot be null or empty.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            var postDeleted = await postRepository.IsPostDeleted(postId);
            if (postDeleted)
            {
                response.Errors.Add("Post deleted or doesn't exist");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            var deleted = await reactionRepository.DeleteAllPostReactions(postId);
            if (!deleted)
            {
                response.Errors.Add("No reactions found for the given post ID.");
                response.ErrorType = ErrorType.NotFound;
                return response;
            }
            response.Message = "Reactions deleted successfully.";
            return response;
        }

        public async Task<ResponseWrapper<List<string>>> FilterPostsReactedByUserAsync(List<string> postIds, string userId)
        {
            var response = new ResponseWrapper<List<string>>();
            if (postIds == null || postIds.Count == 0)
            {
                response.Errors.Add("Post IDs cannot be null or empty.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            if (string.IsNullOrEmpty(userId))
            {
                response.Errors.Add("User ID cannot be null or empty.");
                response.ErrorType = ErrorType.UnAuthorized;
                return response;
            }
            var Ids = await reactionRepository.FilterPostsReactedByUserAsync(postIds, userId);
            response.Data = Ids ?? new List<string>();
            response.Message = "Filtered posts reacted by user successfully.";
            return response;
        }

        public async Task<PaginationResponseWrapper<List<string>>> GetPostsReactedByUserAsync(string userId, string nextReactIdHash)
        {
            var response = new PaginationResponseWrapper<List<string>>();
            if (string.IsNullOrEmpty(userId))
            {
                response.Errors.Add("User ID cannot be null or empty.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            string lastSeenId = string.IsNullOrWhiteSpace(nextReactIdHash) ? "" : PaginationHelper.DecodeCursor(nextReactIdHash!);
            var reactionList = (await reactionRepository.GetPostsReactedByUserAsync(userId, lastSeenId)).ToList();
            bool hasMore = reactionList.Count > (paginationSetting.Value.DefaultPageSize - 1);
            var lastId = hasMore ? reactionList.Last().Id : null;
            response.Data = reactionList.Select(r => r.Id).ToList();
            response.HasMore = hasMore;
            response.Next = lastId != null ? PaginationHelper.GenerateCursor(lastId) : null;

            response.Message = "Posts reacted by user retrieved successfully.";
            return response;
        }

        public async Task<ResponseWrapper<List<string>>> GetUserIdsReactedToPostAsync(string postId)
        {
            var response = new ResponseWrapper<List<string>>();
            if (string.IsNullOrEmpty(postId))
            {
                response.Errors.Add("Post ID cannot be null or empty.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            var postDeleted = await postRepository.IsPostDeleted(postId);
            if (postDeleted)
            {
                response.Errors.Add("Post deleted or doesn't exist");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            var userIds = await reactionRepository.GetUserIdsReactedToPostAsync(postId);
            response.Data = userIds;
            response.Message = "User IDs retrieved successfully.";
            return response;
        }

        public async Task<PaginationResponseWrapper<List<string>>> GetUserIdsReactedToPostAsync(string postId, string next, int take)
        {
            var response = new PaginationResponseWrapper<List<string>>();
            if (string.IsNullOrEmpty(postId))
            {
                response.Errors.Add("Post ID cannot be null or empty.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            string lastSeenId = string.IsNullOrWhiteSpace(next) ? "" : PaginationHelper.DecodeCursor(next);
            var postDeleted = await postRepository.IsPostDeleted(postId);
            if (postDeleted)
            {
                response.Errors.Add("Post deleted or doesn't exist");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            var userIds = await reactionRepository.GetUserIdsReactedToPostAsync(postId, lastSeenId, take + 1);
            bool hasMore = userIds.Count > take;
            string nextCursor = hasMore ? PaginationHelper.GenerateCursor(userIds[take]) : null;
            if (hasMore) userIds = userIds.Take(take).ToList();
            response.Data = userIds;
            response.HasMore = hasMore;
            response.Next = nextCursor;
            response.Message = "User IDs retrieved successfully.";
            return response;
        }

        public async Task<ResponseWrapper<bool>> IsPostReactedByUserAsync(string postId, string userId)
        {
            var response = new ResponseWrapper<bool>();
            if (string.IsNullOrEmpty(postId))
            {
                response.Errors.Add("Post ID cannot be null or empty.");
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
                var postDeleted = await postRepository.IsPostDeleted(postId);
                if (postDeleted)
                {
                    response.Errors.Add("Post deleted or doesn't exist");
                    response.ErrorType = ErrorType.BadRequest;
                    return response;
                }
                var isReacted = await reactionRepository.IsPostReactedByUserAsync(postId, userId);
                response.Data = isReacted;
                response.Message = "Checked if post is reacted by user successfully.";
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