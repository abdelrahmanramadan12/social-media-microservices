namespace Application.Interfaces.Listeners
{
    internal interface IFollowListener : IAsyncDisposable
    {
        Task InitializeAsync();
        Task ListenAsync(CancellationToken cancellationToken);
    }
}
