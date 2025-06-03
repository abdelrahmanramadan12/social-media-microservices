using Domain.Events;

namespace Application.Interfaces.Listeners
{
    interface IMessageNotificationService
    {
        public Task UpdatMessageListNotification(MessageEvent messageEvent);
    }
}
