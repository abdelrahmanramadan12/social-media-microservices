namespace Application.Interfaces
{
    public interface IQueuePublisher<T> : IAsyncDisposable
    {
        Task InitializeAsync();
        Task PublishAsync(T args);
    }
}
