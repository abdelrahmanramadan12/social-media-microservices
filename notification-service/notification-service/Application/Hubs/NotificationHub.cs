using Application.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace Application.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly IConnectionManager _connectionManager;

        public NotificationHub(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext()?.Request.Query["userId"].FirstOrDefault();

            if (!string.IsNullOrEmpty(userId))
            {
                await _connectionManager.AddConnectionAsync(userId, Context.ConnectionId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.GetHttpContext()?.Request.Query["userId"].FirstOrDefault();

            if (!string.IsNullOrEmpty(userId))
            {
                await _connectionManager.RemoveConnectionAsync(userId, Context.ConnectionId);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
