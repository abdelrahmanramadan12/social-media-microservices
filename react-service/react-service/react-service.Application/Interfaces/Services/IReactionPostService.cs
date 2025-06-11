using react_service.Application.DTO;
using react_service.Application.DTO.Reaction.Request.Post;
using react_service.Application.DTO.Reaction.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Application.Interfaces.Services
{
    public interface IReactionPostService
    {
        Task<ResponseWrapper<bool>> DeleteReactionAsync(string postId, string userId);
        Task<ResponseWrapper<bool>> AddReactionAsync(CreatePostReactionRequest reaction, string userId);
        Task<ResponseWrapper<bool>> DeleteReactionsByPostId(string postId);
        Task<ResponseWrapper<List<string>>> FilterPostsReactedByUserAsync(List<string> postIds, string userId);
        Task<PaginationResponseWrapper<List<string>>> GetPostsReactedByUserAsync(string userId, string? nextReactIdHash);
        Task<ResponseWrapper<List<string>>> GetUserIdsReactedToPostAsync(string postId);
        Task<PaginationResponseWrapper<List<string>>> GetUserIdsReactedToPostAsync(string postId, string next, int take);
        Task<ResponseWrapper<bool>> IsPostReactedByUserAsync(string postId, string userId);
    }
}
