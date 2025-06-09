using Application.Abstractions;
using Domain.Entities;

namespace Application.Services
{
    public class FeedQueryService : IFeedQueryService
    {
        private readonly IFeedRepository _feedRepository;

        public FeedQueryService(IFeedRepository feedRepository)
        {
            _feedRepository = feedRepository;
        }

        public async Task<List<Post>> GetUserTimeline(string userId)
        {
            var feedRes = await _feedRepository.FindUserFeedAsync(userId);

            if (!feedRes.Success)
            {
                return new List<Post>();
            }
            else
            {
                return feedRes.Value.Timeline;
            }
        }
    }
}
