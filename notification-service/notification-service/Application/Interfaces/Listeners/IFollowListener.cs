namespace Application.Interfaces.Listeners
{
    public interface IFollowListener : IAsyncDisposable
    {
        Task InitializeAsync();
        public void ListenAsync(CancellationToken cancellationToken);
    }
}
