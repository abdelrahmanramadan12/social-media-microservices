namespace Service.Interfaces.RabbitMqServices
{
    public interface IFollowListener : IAsyncDisposable
    {
        Task InitializeAsync();
        Task ListenAsync(CancellationToken cancellationToken);
    }
}
