namespace Service.Interfaces.RabbitMQServices
{
    public interface IRabbitMqPublisher
    {
        Task PublishAsync<T>(T message, List<string> queueNames, CancellationToken ct = default);

    }
}
