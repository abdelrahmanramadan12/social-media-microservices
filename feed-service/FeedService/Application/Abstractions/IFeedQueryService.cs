using Domain.Entities;

namespace Application.Abstractions
{
    public interface IFeedQueryService
    {
        Task<List<Post>> GetUserTimeline(string userId);
    }
}