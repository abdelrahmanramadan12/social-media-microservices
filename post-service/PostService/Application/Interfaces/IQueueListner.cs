using Application.Events;

namespace Application.Interfaces
{
    public interface IQueueListener<T> : IAsyncDisposable where T : QueueEvent
    {
        Task InitializeAsync();
        Task ListenAsync(CancellationToken _cancellationToken);
    }
}
