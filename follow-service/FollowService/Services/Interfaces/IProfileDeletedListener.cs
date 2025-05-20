namespace Services.Interfaces
{
    public interface IProfileDeletedListener : IAsyncDisposable
    {
        Task InitializeAsync();
        Task ListenAsync();
    }
}
