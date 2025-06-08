using Application.Abstractions;
using Application.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs
{
    public class RealtimeMessenger : IRealtimeMessenger
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IHubContext<ChatHub> _hubContext;

        public RealtimeMessenger(IHubContext<ChatHub> hubContext, IConnectionManager connectionManager)
        {
            _hubContext = hubContext;
            _connectionManager = connectionManager;
        }

        public async Task SendMessageAsync(string userId, MessageDTO message)
        {
            var connections = await _connectionManager.GetConnectionsAsync(userId);
            foreach (var connId in connections)
            {
                await _hubContext.Clients.Client(connId).SendAsync("ReceivePrivateMessage", message);
            }
        }
    }
}
