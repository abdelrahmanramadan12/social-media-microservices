using react_service.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Application.Interfaces.Repositories
{
    public interface IPostReactionRepository
    {
        public Task<List<string>> FilterPostsReactedByUserAsync(List<string> postIds, string userId);
        Task<List<PostReaction>> GetReactsOfPostAsync(string postId, string nextReactIdHash);
        Task<List<PostReaction>> GetPostsReactedByUserAsync(string userId, string nextReactIdHash);
        Task<bool> DeleteReactionAsync(string postId, string userId);
        Task<string> AddReactionAsync(PostReaction reaction);
        Task<bool> DeleteAllPostReactions(string postId);
        Task<List<string>> GetUserIdsReactedToPostAsync(string postId);
        Task<List<string>> GetUserIdsReactedToPostAsync(string postId, string lastSeenId, int take);
        Task<bool> IsPostReactedByUserAsync(string postId, string userId);
    }
}
