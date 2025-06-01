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

namespace react_service.Application.Services
{
    public class ReactionPostService : IReactionPostService
    {
        private readonly HttpClient _httpClient;
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

        public async Task<PagedReactsResponse> GetReactsOfPostAsync(string postId, string? nextReactIdHash)
        {
            string lastSeenId;
            if (nextReactIdHash == null || nextReactIdHash?.Trim() == "")
            {
                lastSeenId = "";
            }else
            {
                lastSeenId = PaginationHelper.DecodeCursor(nextReactIdHash!);
            }

            var reactionList = (await reactionRepository.GetReactsOfPostAsync(postId, lastSeenId)).ToList();

            bool hasMore = reactionList.Count > (paginationSetting.Value.DefaultPageSize - 1);

            var response = mapper.Map<PagedReactsResponse>(reactionList);

            var lastId = hasMore ? reactionList.Last().Id : null;
            response.HasMore = hasMore;
            response.NextCursor = lastId != null ? PaginationHelper.GenerateCursor(lastId) : null;

            return response;
        }

        public async Task<string> AddReactionAsync(CreateReactionRequest reaction, string userId)
        {
            #region validation
            // validation on postId 
            // var UserId =  _gatewayService.CallServiceAsync<string>("UserService", "/api/public/user/validateUserId?userId=" + userId);
            // validation on UserId
            // var postId = _gatewayService.CallServiceAsync<string>("PostService", "/api/public/post/validatePostId?postId=" + postId);
            #endregion

           
            var reactionObj = mapper.Map<ReactionPost>(reaction);
            //var newReaction = new ReactionPublishDTO { PostId = reation.PostId, ReactorId = userId, EventType = EventType.ReactionPostCreated };
            //reactionPublisher.Publish(newReaction);
            reactionObj.UserId = userId;
            return await reactionRepository.AddReactionAsync(reactionObj);
        }

        public async Task<bool> DeleteReactionAsync(string postId, string userId)
        {
            #region validation
            // validation on postId 

            // var UserId =  _gatewayService.CallServiceAsync<string>("UserService", "/api/public/user/validateUserId?userId=" + userId);

            // validation on UserId

            // var postId = _gatewayService.CallServiceAsync<string>("PostService", "/api/public/post/validatePostId?postId=" + postId);
            #endregion

            var newReaction = new ReactionPublishDTO { PostId = postId, ReactorId = userId, EventType = EventType.ReactionPostDeleted };
           // reactionPublisher.Publish(newReaction);
            return await reactionRepository.DeleteReactionAsync(postId, userId);


        }
        public async Task<PagedReactsResponse> GetPostsReactedByUserAsync(string userId, string? nextReactIdHash)
        {
            string lastSeenId;
            if (nextReactIdHash == null || nextReactIdHash?.Trim() == "")
            {
                lastSeenId = "";
            }
            else
            {
                lastSeenId = PaginationHelper.DecodeCursor(nextReactIdHash!);
            }

            var reactionList = (await reactionRepository.GetPostsReactedByUserAsync(userId, lastSeenId)).ToList();

            bool hasMore = reactionList.Count > (paginationSetting.Value.DefaultPageSize - 1);

            var response = mapper.Map<PagedReactsResponse>(reactionList);

            var lastId = hasMore ? reactionList.Last().Id : null;
            response.HasMore = hasMore;
            response.NextCursor = lastId != null ? PaginationHelper.GenerateCursor(lastId) : null;

            return response;
        }

        public async Task<bool> DeleteReactionsByPostId(string postId)
        {
            #region validation
            // validation on postId 
            // var UserId =  _gatewayService.CallServiceAsync<string>("UserService", "/api/public/user/validateUserId?userId=" + userId);
            // validation on UserId
            // var postId = _gatewayService.CallServiceAsync<string>("PostService", "/api/public/post/validatePostId?postId=" + postId);
            #endregion
            return await reactionRepository.DeleteReactionsByPostId(postId);


        }
        public async Task<PostsReactedByUserDTO> FilterPostsReactedByUserAsync(List<string> postIds, string userId)
        {
            #region validation
            // validation on postId 
            // var UserId =  _gatewayService.CallServiceAsync<string>("UserService", "/api/public/user/validateUserId?userId=" + userId);
            // validation on UserId
            // var postId = _gatewayService.CallServiceAsync<string>("PostService", "/api/public/post/validatePostId?postId=" + postId);
            #endregion
            var obj = await reactionRepository.FilterPostsReactedByUserAsync(postIds, userId);
            return new PostsReactedByUserDTO
            {
                postIds = obj
            };

        }
    }
}