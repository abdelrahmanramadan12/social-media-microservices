using Domain.Events;

namespace Application.Interfaces.Services
{

    interface IMessageNotificationService
    {
        public Task UpdatMessageListNotification(MessageEvent messageEvent);
    }

}
