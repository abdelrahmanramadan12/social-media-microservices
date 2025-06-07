namespace Application.Abstractions
{
    public interface IRealtimeMessenger
    {
        Task SendMessageAsync(string userId, string message);
    }
}
