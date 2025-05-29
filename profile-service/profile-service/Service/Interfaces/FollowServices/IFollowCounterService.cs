using Domain.Events;

namespace Service.Interfaces.FollowServices
{
    public interface IFollowCounterService
    {
        Task UpdateCounter(string followerId, string followedId, FollowEventType followType);
    }
}
