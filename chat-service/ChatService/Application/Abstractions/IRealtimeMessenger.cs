using Application.DTOs;

namespace Application.Abstractions
{
    public interface IRealtimeMessenger
    {
        Task SendMessageAsync(string userId, MessageDTO message);
    }
}
