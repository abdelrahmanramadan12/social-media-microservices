using Application.DTO;
using Domain.CacheEntities;
using Domain.Events;

namespace Application.Interfaces.Services
{
    interface IFollowNotificationService
    {
        Task UpdateFollowersListNotification(FollowEvent followedDTO);

        Task RemoveFollowerFromNotificationList(FollowEvent followedDTO);

    }
}
