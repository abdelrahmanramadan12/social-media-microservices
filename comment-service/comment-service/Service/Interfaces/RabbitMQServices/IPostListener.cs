namespace Service.Interfaces.RabbitMQServices
{
    public interface IPostListener:IAsyncDisposable
    {
        Task InitializeAsync();
        Task ListenAsync(CancellationToken cancellationToken);
    }
}
