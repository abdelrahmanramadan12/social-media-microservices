using Application.Events;

namespace Application.Abstractions
{
    public interface IQueueListener<T> : IAsyncDisposable where T : QueueEvent
    {
        Task InitializeAsync();
        Task ListenAsync(CancellationToken _cancellationToken);
    }
}