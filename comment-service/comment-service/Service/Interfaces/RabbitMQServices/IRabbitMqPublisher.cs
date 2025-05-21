namespace Service.Interfaces.RabbitMQServices
{
    public interface IRabbitMqPublisher
    {
        Task PublishAsync<T>(T message, string queueName, CancellationToken ct = default);

    }
}
