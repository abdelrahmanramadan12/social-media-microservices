namespace Application.Abstractions
{
    public interface IQueueListener<T> : IAsyncDisposable
    {
        Task InitializeAsync();
        Task ListenAsync(CancellationToken _cancellationToken);
    }
}
