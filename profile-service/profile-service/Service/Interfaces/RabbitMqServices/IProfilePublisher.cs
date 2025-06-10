using Domain.Events;

namespace Service.Interfaces.RabbitMqServices
{
    public interface IProfilePublisher
    {
        Task PublishAsync(ProfileEvent evt, CancellationToken ct = default);
    }
}
