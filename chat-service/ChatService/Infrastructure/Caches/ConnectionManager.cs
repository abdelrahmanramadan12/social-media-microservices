using Application.Abstractions;
using StackExchange.Redis;

namespace Infrastructure.Caches
{
    public class ConnectionManager : IConnectionManager
    {
        private readonly IDatabase _db;
        private const string Prefix = "user_connections:";

        public ConnectionManager(IDatabase db)
        {
            _db = db;
        }

        public async Task AddConnectionAsync(string userId, string connectionId) =>
            await _db.SetAddAsync($"{Prefix}{userId}", connectionId);

        public async Task RemoveConnectionAsync(string userId, string connectionId) =>
            await _db.SetRemoveAsync($"{Prefix}{userId}", connectionId);

        public async Task<IEnumerable<string>> GetConnectionsAsync(string userId)
        {
            var result = await _db.SetMembersAsync($"{Prefix}{userId}");
            return result.Select(x => x.ToString());
        }
    }
}
