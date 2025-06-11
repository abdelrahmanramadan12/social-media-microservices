using Domain.Events;

namespace Application.Interfaces.Services
{
    interface IReactionNotificationService
    {
        Task UpdateReactionsListNotification(ReactionEvent ReactionEventDTO);

        Task RemovReactionsFromNotificationList(ReactionEvent ReactionEventDTO);

    }
}
