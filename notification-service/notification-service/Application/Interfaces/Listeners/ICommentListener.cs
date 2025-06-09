namespace Application.Interfaces.Listeners
{
    public interface ICommentListener : IAsyncDisposable
    {

        Task InitializeAsync();
        Task ListenAsync(CancellationToken cancellationToken);
    }
}
