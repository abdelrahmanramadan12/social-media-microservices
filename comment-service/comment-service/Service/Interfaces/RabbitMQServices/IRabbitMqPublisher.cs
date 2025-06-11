namespace Service.Interfaces.RabbitMQServices
{
    public interface IRabbitMqPublisher : IAsyncDisposable
    {
        Task PublishAsync<T>(T message, List<string> queueNames, CancellationToken ct = default);

    }
}
