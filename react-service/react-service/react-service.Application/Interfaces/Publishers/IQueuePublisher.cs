using react_service.Application.Events;

namespace react_service.Application.Interfaces.Publishers
{
    public interface IQueuePublisher<T> : IAsyncDisposable where T : QueueEvent
    {
        Task InitializeAsync();
        Task PublishAsync(T args);
    }
}
