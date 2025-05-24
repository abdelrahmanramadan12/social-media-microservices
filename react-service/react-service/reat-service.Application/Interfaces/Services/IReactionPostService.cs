using reat_service.Application.DTO.ReactionPost.Request;
using reat_service.Application.DTO.ReactionPost.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reat_service.Application.Interfaces.Services
{
    public interface IReactionPostService
    {
      Task<PagedReactsResponse> GetReactsByPostAsync(string postId, string? nextReactIdHash, string userId);
       Task<bool> DeleteReactionAsync(string postId, string userId);
       Task<string> AddReactionAsync(CreateReactionRequest reaction , string userId);
        Task<bool> DeleteReactionsByPostId(string postId);
    }
}
