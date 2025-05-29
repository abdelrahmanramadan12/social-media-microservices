using Application.Events;

namespace Application.Abstractions
{
    public interface IQueuePublisher<T> : IAsyncDisposable where T : QueueEvent
    {
        Task InitializeAsync();
        Task PublishAsync(T args);
    }
}
