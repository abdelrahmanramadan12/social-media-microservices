using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using react_service.Domain.Entites;
using react_service.Domain.Enums;
using react_service.Application.DTO.RabbitMQ;
using react_service.Application.DTO.ReactionPost.Request;
using react_service.Application.DTO.ReactionPost.Response;
using react_service.Application.Helpers;
using react_service.Application.Interfaces.Publishers;
using react_service.Application.Interfaces.Repositories;
using react_service.Application.Interfaces.Services;
using react_service.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using react_service.Application.DTO;

namespace react_service.Application.Services
{
    public class ReactionPostService : IReactionPostService
    {
        // private readonly HttpClient _httpClient; // Remove or comment out this unused field
        private readonly IReactionPostRepository reactionRepository;
        private readonly IMapper mapper;
        private readonly IOptions<PaginationSettings> paginationSetting;
        private readonly IReactionPublisher reactionPublisher;

        public ReactionPostService(IReactionPostRepository reactionRepository, IMapper mapper, IOptions<PaginationSettings> paginationSetting
            , IReactionPublisher reactionPublisher)
        {
            this.reactionRepository = reactionRepository;
            this.mapper = mapper;
            this.paginationSetting = paginationSetting;
            this.reactionPublisher = reactionPublisher;
        }

        public async Task<ResponseWrapper<PagedReactsResponse>> GetReactsOfPostAsync(string postId, string? nextReactIdHash)
        {
            var response = new ResponseWrapper<PagedReactsResponse>();
            if (string.IsNullOrEmpty(postId))
            {
                response.Errors.Add("Post ID cannot be null or empty.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            string lastSeenId = string.IsNullOrWhiteSpace(nextReactIdHash) ? "" : PaginationHelper.DecodeCursor(nextReactIdHash!);
            var reactionList = (await reactionRepository.GetReactsOfPostAsync(postId, lastSeenId)).ToList();
            bool hasMore = reactionList.Count > (paginationSetting.Value.DefaultPageSize - 1);
            var pagedResponse = mapper.Map<PagedReactsResponse>(reactionList);
            var lastId = hasMore ? reactionList.Last().Id : null;
            pagedResponse.HasMore = hasMore;
            pagedResponse.Next = lastId != null ? PaginationHelper.GenerateCursor(lastId) : null;
            response.Data = pagedResponse;
            response.Message = "Reactions retrieved successfully.";
            return response;
        }

        public async Task<ResponseWrapper<object>> DeleteReactionAsync(string postId, string userId)
        {
            var response = new ResponseWrapper<object>();
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
            var deleted = await reactionRepository.DeleteReactionAsync(postId, userId);
            if (!deleted)
            {
                response.Errors.Add("Reaction not found.");
                response.ErrorType = ErrorType.NotFound;
                return response;
            }
            response.Message = "Reaction deleted successfully.";
            return response;
        }

        public async Task<ResponseWrapper<object>> AddReactionAsync(CreateReactionRequest reaction, string userId)
        {
            var response = new ResponseWrapper<object>();
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
            var reactionObj = mapper.Map<ReactionPost>(reaction);
            reactionObj.UserId = userId;
            await reactionRepository.AddReactionAsync(reactionObj);
            response.Message = "Reaction added successfully.";
            return response;
        }

        public async Task<ResponseWrapper<object>> DeleteReactionsByPostId(string postId)
        {
            var response = new ResponseWrapper<object>();
            if (string.IsNullOrEmpty(postId))
            {
                response.Errors.Add("Post ID cannot be null or empty.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            var deleted = await reactionRepository.DeleteReactionsByPostId(postId);
            if (!deleted)
            {
                response.Errors.Add("No reactions found for the given post ID.");
                response.ErrorType = ErrorType.NotFound;
                return response;
            }
            response.Message = "Reactions deleted successfully.";
            return response;
        }

        public async Task<ResponseWrapper<PostsReactedByUserDTO>> FilterPostsReactedByUserAsync(List<string> postIds, string userId)
        {
            var response = new ResponseWrapper<PostsReactedByUserDTO>();
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
            var obj = await reactionRepository.FilterPostsReactedByUserAsync(postIds, userId);
            response.Data = new PostsReactedByUserDTO { postIds = obj };
            response.Message = "Filtered posts reacted by user successfully.";
            return response;
        }

        public async Task<ResponseWrapper<PagedReactsResponse>> GetPostsReactedByUserAsync(string userId, string? nextReactIdHash)
        {
            var response = new ResponseWrapper<PagedReactsResponse>();
            if (string.IsNullOrEmpty(userId))
            {
                response.Errors.Add("User ID cannot be null or empty.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }
            string lastSeenId = string.IsNullOrWhiteSpace(nextReactIdHash) ? "" : PaginationHelper.DecodeCursor(nextReactIdHash!);
            var reactionList = (await reactionRepository.GetPostsReactedByUserAsync(userId, lastSeenId)).ToList();
            bool hasMore = reactionList.Count > (paginationSetting.Value.DefaultPageSize - 1);
            var pagedResponse = mapper.Map<PagedReactsResponse>(reactionList);
            var lastId = hasMore ? reactionList.Last().Id : null;
            pagedResponse.HasMore = hasMore;
            pagedResponse.Next = lastId != null ? PaginationHelper.GenerateCursor(lastId) : null;
            response.Data = pagedResponse;
            response.Message = "Posts reacted by user retrieved successfully.";
            return response;
        }
    }
}