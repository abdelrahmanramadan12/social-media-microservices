namespace Application.Interfaces.Listeners
{
    public interface IReactionListener : IAsyncDisposable
    {
        Task InitializeAsync();
        Task ListenAsync(CancellationToken cancellationToken);
    }
}
