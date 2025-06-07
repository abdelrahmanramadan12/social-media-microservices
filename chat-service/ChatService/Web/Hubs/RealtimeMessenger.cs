using Application.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs
{
    public class RealtimeMessenger : IRealtimeMessenger
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public RealtimeMessenger(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }
        // to be fixed
        public Task SendMessageAsync(string userId, string message)
        {
            return _hubContext.Clients.User(userId).SendAsync("ReceiveMessage", message);
        }
    }
}
