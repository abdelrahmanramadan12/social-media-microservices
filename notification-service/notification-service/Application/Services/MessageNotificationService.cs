using Microsoft.AspNetCore.SignalR;
using Application.Hubs;
using Application.Interfaces.Listeners;
using Application.Interfaces;
using Domain.CoreEntities;
using Domain.Events;

namespace Application.Services
{
    public class MessageNotificationService(IUnitOfWork unitOfWork, IHubContext<MessageNotificationHub> hubContext)
                                            : IMessageNotificationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IHubContext<MessageNotificationHub> _hubContext = hubContext;

        public async Task UpdatMessageListNotification(MessageEvent messageEvent)
        {
            var msg = _unitOfWork.CoreRepository<Messages>().GetSingleAsync(x => x.RevieverId == messageEvent.ReceiverId).Result
                                                                            ?? throw new Exception("Error updating message");

            var messages = msg.MessageList.Where(x => x.Key == messageEvent.SenderId).FirstOrDefault();

            var message = messages.Value.Where(x => x.Id == messageEvent.MessageId).FirstOrDefault();
            message.IsRead = true;

            await _unitOfWork.CoreRepository<Messages>().AddAsync(msg);
            await _unitOfWork.SaveChangesAsync();

            // Notify the receiver in real-time
            await _hubContext.Clients.User(messageEvent.ReceiverId).SendAsync("ReceiveMessageNotification", new
            {
                MessageId = message.Id,
                SenderId = messages.Key,
                Timestamp = message.SentAt
            });
        }
    }
}