namespace Application.Interfaces.Services
{
    public interface IConnectionManager
    {
        Task AddConnectionAsync(string userId, string connectionId);
        Task RemoveConnectionAsync(string userId, string connectionId);
        Task<IEnumerable<string>> GetConnectionsAsync(string userId);
    }
}
