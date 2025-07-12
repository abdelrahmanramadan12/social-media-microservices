using Application.DTO;

namespace Application.Interfaces.Services
{
    public interface IRealtimeNotifier
    {
        Task SendMessageAsync(string userId, NotificationsDTO notification);
    }
}
