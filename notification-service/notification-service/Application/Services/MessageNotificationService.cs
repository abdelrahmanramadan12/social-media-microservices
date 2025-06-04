using Microsoft.AspNetCore.SignalR;
using Application.Hubs;
using Application.Interfaces.Listeners;
using Application.Interfaces;
using Domain.CoreEntities;
using Domain.Events;

public class MessageNotificationService(IUnitOfWork unitOfWork, IHubContext<MessageNotificationHub> hubContext)
    : IMessageNotificationService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IHubContext<MessageNotificationHub> _hubContext = hubContext;

    public async Task UpdatMessageListNotification(MessageEvent messageEvent)
    {
        Messages newMessage = new()
        {
            Id = messageEvent.MessageId ?? Guid.NewGuid().ToString(),
            SourceUserId = messageEvent.SenderId ?? "",
            DestinationUserId = messageEvent.ReceiverId ?? "",
            IsRead = false,
            SentAt = messageEvent.Timestamp
        };

        await _unitOfWork.CoreRepository<Messages>().AddAsync(newMessage);
        await _unitOfWork.SaveChangesAsync();

        // Notify the receiver in real-time
        await _hubContext.Clients.User(messageEvent.ReceiverId).SendAsync("ReceiveMessageNotification", new
        {
            MessageId = newMessage.Id,
            SenderId = newMessage.SourceUserId,
            Timestamp = newMessage.SentAt
        });
    }
}
