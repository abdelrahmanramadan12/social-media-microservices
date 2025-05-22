using Domain.Entities;
using MongoDB.Bson;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IFollowRepository
    {
        Task<Follow> FindAsync(string followerId, string followingId);
        Task<ICollection<Follow>> FindFollowersAsync(string followingId, string? cursor = null, int? pageSize = null);
        Task<ICollection<Follow>> FindFollowingAsync(string followerId, string? cursor = null, int? pageSize = null);
        Task<Follow> AddAsync(Follow follow);
        Task DeleteAsync(ObjectId id);
    }
}
