using Domain.Events;
using Service.DTOs.Responses;

namespace Service.Interfaces.FollowServices
{
    public interface IFollowCounterService
    {
        public Task UpdateCounter(string followerId, string followedId, FollowEventType followType);
    }
}
