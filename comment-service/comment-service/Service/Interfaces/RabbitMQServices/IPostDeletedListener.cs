namespace Service.Interfaces.RabbitMQServices
{
    public interface IPostDeletedListener : IAsyncDisposable
    {
        Task InitializeAsync();
        Task ListenAsync(CancellationToken cancellationToken);

    }
}
