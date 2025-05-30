using react_service.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Application.Interfaces.Repositories
{
    public interface IReactionPostRepository
    {
        Task<List<ReactionPost>> GetReactsOfPostAsync(string postId, string  nextReactIdHash);
        Task<string> CreateReaction(ReactionPost reaction);
        Task<bool> DeleteReactionAsync(string postId, string userId);
        public Task<bool> DeleteReactionsByPostId(string postId);

        public Task<List<string>> FilterPostsReactedByUserAsync(List<string> postIds, string userId);


    }
}
