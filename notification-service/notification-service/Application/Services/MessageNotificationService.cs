using Application.Interfaces;
using Application.Interfaces.Listeners;
using Domain.CoreEntities;
using Domain.Events;

namespace Application.Services
{
    public class MessageNotificationService(IUnitOfWork unitOfWork) : IMessageNotificationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task UpdatMessageListNotification(MessageEvent messageEvent)
        {
            var message = await _unitOfWork.CoreRepository<Messages>().GetSingleAsync(Id => Id.Id == messageEvent.MessageId);
            if (message == null)
                return;
            Messages NewMessage = new()
            {
                Id = messageEvent.MessageId ?? "",
                SourceUserId = messageEvent.SenderId ?? "",
                DestinationUserId = messageEvent.ReceiverId ?? "",
                IsRead = false,
                SentAt =messageEvent.Timestamp
            };

            await _unitOfWork.CoreRepository<Messages>().AddAsync(message);

        }
    }
}
