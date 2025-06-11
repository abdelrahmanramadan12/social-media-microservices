namespace Application.Interfaces.Listeners
{
    public interface IFollowListener : IAsyncDisposable
    {
        Task InitializeAsync();
        Task ListenAsync(CancellationToken cancellationToken);
    }
}
