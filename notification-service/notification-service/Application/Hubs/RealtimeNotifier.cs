using Application.Interfaces.Services;
using Application.DTO;
using Microsoft.AspNetCore.SignalR;
using Application.Hubs;

namespace Web.Hubs
{
    public class RealtimeNotifier : IRealtimeNotifier
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IHubContext<NotificationHub> _hubContext;

        public RealtimeNotifier(IHubContext<NotificationHub> hubContext, IConnectionManager connectionManager)
        {
            _hubContext = hubContext;
            _connectionManager = connectionManager;
        }

        public async Task SendMessageAsync(string userId, NotificationsDTO notification)
        {
            var connections = await _connectionManager.GetConnectionsAsync(userId);
            if (connections != null)
            {
                foreach (var connId in connections)
                {
                    await _hubContext.Clients.Client(connId).SendAsync("ReceiveNotification", notification);
                }
            }
        }
    }
}
