using Application.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IAuthServiceClient _authServiceClient;

        public ChatHub(IConnectionManager connectionManager, IAuthServiceClient authServiceClient   )
        {
            _connectionManager = connectionManager;
            _authServiceClient = authServiceClient;
        }

        public override async Task OnConnectedAsync()
        {
            var token = Context.GetHttpContext()?.Request.Query["token"];

            if (!string.IsNullOrEmpty(token))
            {
                var response = await _authServiceClient.VerifyTokenAsync(token);

                if (response.Success && response.Value != null && !String.IsNullOrEmpty(response.Value.UserId))
                {
                    await _connectionManager.AddConnectionAsync(response.Value.UserId, Context.ConnectionId);
                }
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var token = Context.GetHttpContext()?.Request.Query["token"];

            if (!string.IsNullOrEmpty(token))
            {
                var response = await _authServiceClient.VerifyTokenAsync(token);

                if (response.Success && response.Value != null && !String.IsNullOrEmpty(response.Value.UserId))
                {
                    await _connectionManager.RemoveConnectionAsync(response.Value.UserId, Context.ConnectionId);
                }
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendPrivateMessage(string toUserId, string message)
        {
            var connections = await _connectionManager.GetConnectionsAsync(toUserId);
            if (connections != null)
            {
                foreach (var connId in connections)
                {
                    await Clients.Client(connId).SendAsync("ReceivePrivateMessage", message);
                }
            }
        }
    }
}
