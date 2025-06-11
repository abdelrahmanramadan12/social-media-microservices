namespace Service.Interfaces.RabbitMQServices
{
    public interface IQueueListener<T> :IAsyncDisposable
    {
        Task InitializeAsync();
        Task ListenAsync(CancellationToken cancellationToken);
    }
}
