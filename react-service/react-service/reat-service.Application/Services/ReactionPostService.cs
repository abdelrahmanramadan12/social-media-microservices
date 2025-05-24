using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using react_service.Domain.Entites;
using reat_service.Application.DTO.ReactionPost.Request;
using reat_service.Application.DTO.ReactionPost.Response;
using reat_service.Application.Helpers;
using reat_service.Application.Interfaces.Repositories;
using reat_service.Application.Interfaces.Services;
using reat_service.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reat_service.Application.Services
{
    public  class ReactionPostService : IReactionPostService
    {
        private readonly HttpClient _httpClient;
        private readonly IReactionPostRepository reactionRepository;
        private readonly IMapper mapper;
        private readonly IOptions<PaginationSettings> paginationSetting;

        public ReactionPostService( IReactionPostRepository reactionRepository, IMapper mapper , IOptions<PaginationSettings> paginationSetting)
        {
            this.reactionRepository = reactionRepository;
            this.mapper = mapper;
            this.paginationSetting = paginationSetting;
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

            var reactionList = await  reactionRepository.GetReactsByPostAsync(postId, nextReactIdHash, userId);
           
            bool hasMore =  reactionList.Count() > (paginationSetting.Value.DefaultPageSize - 1) ; 
           
            var response=  mapper.Map<PagedReactsResponse>(reactionList);
            
            response.HasMore = hasMore;
            response.NextCursor = hasMore ? reactionList.LastOrDefault().Id : null;


            #region AddProfileImageAndUserName   
            #endregion

            return response;



        }
        public async Task<string> AddReactionAsync(CreateReactionRequest reation ,string  userId)
        {
            #region validation
            // validation on postId 
            // var UserId =  _gatewayService.CallServiceAsync<string>("UserService", "/api/public/user/validateUserId?userId=" + userId);
            // validation on UserId
            // var postId = _gatewayService.CallServiceAsync<string>("PostService", "/api/public/post/validatePostId?postId=" + postId);
            #endregion

            
            var reactionObj = mapper.Map<ReactionPost>(reation);
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

            return await reactionRepository.DeleteReactionAsync(postId, userId);

            
        }

     
    }
}
