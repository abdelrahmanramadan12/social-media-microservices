using react_service.Application.DTO;
using react_service.Application.DTO.ReactionPost.Request;
using react_service.Application.DTO.ReactionPost.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Application.Interfaces.Services
{
    public interface IReactionPostService
    {
        Task<ResponseWrapper<object>> DeleteReactionAsync(string postId, string userId);
        Task<ResponseWrapper<object>> AddReactionAsync(CreateReactionRequest reaction, string userId);
        Task<ResponseWrapper<object>> DeleteReactionsByPostId(string postId);
        Task<ResponseWrapper<PostsReactedByUserDTO>> FilterPostsReactedByUserAsync(List<string> postIds, string userId);
        Task<ResponseWrapper<PagedReactsResponse>> GetPostsReactedByUserAsync(string userId, string? nextReactIdHash);
        Task<ResponseWrapper<ReactionsUsersResponse>> GetUserIdsReactedToPostAsync(string postId);
        Task<ResponseWrapper<ReactionsUsersResponse>> GetUserIdsReactedToPostAsync(string postId, string next, int take);
    }
}
