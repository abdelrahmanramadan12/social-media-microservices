using Application.DTOs.Reaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IReactionServiceClient
    {
        Task<ResponseWrapper<List<string>>> FilterPostsReactedByUserAsync(GetPostsReactedByUserRequest request);
        Task<ResponseWrapper<object>> GetPostsReactedByUserAsync(string userId, string? nextReactIdHash = null);
        Task<ResponseWrapper<object>> GetReactsOfPostAsync(GetReactsOfPostRequest request);
    }
}
