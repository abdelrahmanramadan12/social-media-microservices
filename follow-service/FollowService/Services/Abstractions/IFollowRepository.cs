using Domain.Entities;
using MongoDB.Bson;

namespace Application.Abstractions
{
    public interface IFollowRepository
    {
        Task<Follow> FindAsync(string followerId, string followingId);
        Task<ICollection<Follow>> FindFollowersAsync(string followingId, string? cursor = null, int? pageSize = null);
        Task<ICollection<Follow>> FindFollowingAsync(string followerId, string? cursor = null, int? pageSize = null);
        Task<Follow> AddAsync(Follow follow);
        Task DeleteAsync(ObjectId id);
        Task<ICollection<string>> FilterFollowersAsync(string followingId, List<string> followerIds);
        Task<ICollection<string>> FilterFollowingAsync(string followerId, List<string> followingIds);
    }
}
