using react_service.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reat_service.Application.Interfaces.Repositories
{
    public interface IReactionPostRepository
    {
        Task<List<ReactionPost>> GetReactsByPostAsync(string postId, string  nextReactIdHash, string userId);
        Task<string> CreateReaction(ReactionPost reaction);
        Task<bool> DeleteReactionAsync(string postId, string userId);

    }
}
