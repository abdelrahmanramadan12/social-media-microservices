namespace Service.Interfaces
{
    public interface IPostDeletedListener : IAsyncDisposable
    {
        Task InitializeAsync();
        Task ListenAsync(CancellationToken cancellationToken);

    }
}
