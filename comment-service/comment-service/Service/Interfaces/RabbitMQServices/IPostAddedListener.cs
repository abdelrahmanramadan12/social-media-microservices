namespace Service.Interfaces.RabbitMQServices
{
    public interface IPostAddedListener
    {
        Task InitializeAsync();
        Task ListenAsync(CancellationToken cancellationToken);
    }
}
