using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using react_service.Domain.Entites;
using react_service.Domain.Enums;
using reat_service.Application.DTO.RabbitMQ;
using reat_service.Application.DTO.ReactionPost.Request;
using reat_service.Application.DTO.ReactionPost.Response;
using reat_service.Application.Helpers;
using reat_service.Application.Interfaces.Publishers;
using reat_service.Application.Interfaces.Repositories;
using reat_service.Application.Interfaces.Services;
using reat_service.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace reat_service.Application.Services
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

        public async Task<PagedReactsResponse> GetReactsByPostAsync(string postId, string? nextReactIdHash, string userId)
        {

            #region validation
            // validation on postId 

            // var UserId =  _gatewayService.CallServiceAsync<string>("UserService", "/api/public/user/validateUserId?userId=" + userId);

            // validation on UserId

            // var postId = _gatewayService.CallServiceAsync<string>("PostService", "/api/public/post/validatePostId?postId=" + postId);
            #endregion

            //var lastSeenId = PaginationHelper.DecodeCursor(nextReactIdHash);

            var reactionList = await reactionRepository.GetReactsByPostAsync(postId, nextReactIdHash, userId);

            bool hasMore = reactionList.Count() > (paginationSetting.Value.DefaultPageSize - 1);

            var response = mapper.Map<PagedReactsResponse>(reactionList);

            response.HasMore = hasMore;
            response.NextCursor = hasMore ? reactionList.LastOrDefault().Id : null;


            #region AddProfileImageAndUserName   
            #endregion

            return response;



        }
        public async Task<string> AddReactionAsync(CreateReactionRequest reation, string userId)
        {
            #region validation
            // validation on postId 
            // var UserId =  _gatewayService.CallServiceAsync<string>("UserService", "/api/public/user/validateUserId?userId=" + userId);
            // validation on UserId
            // var postId = _gatewayService.CallServiceAsync<string>("PostService", "/api/public/post/validatePostId?postId=" + postId);
            #endregion

           
            var reactionObj = mapper.Map<ReactionPost>(reation);
            //var newReaction = new ReactionPublishDTO { PostId = reation.PostId, ReactorId = userId, EventType = EventType.ReactionPostCreated };
            //reactionPublisher.Publish(newReaction);
            reactionObj.UserId = userId;
            return await reactionRepository.CreateReaction(reactionObj);
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
        public async Task<PostsReactedByUserDTO> IsPostsReactedByUserAsync(List<string> postIds, string userId)
        {
            #region validation
            // validation on postId 
            // var UserId =  _gatewayService.CallServiceAsync<string>("UserService", "/api/public/user/validateUserId?userId=" + userId);
            // validation on UserId
            // var postId = _gatewayService.CallServiceAsync<string>("PostService", "/api/public/post/validatePostId?postId=" + postId);
            #endregion
            var obj = await reactionRepository.IsPostsReactedByUserAsync(postIds, userId);
            return new PostsReactedByUserDTO
            {
                postIds = obj
            };

        }
    }
}