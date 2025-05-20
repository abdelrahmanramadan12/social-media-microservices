namespace Services.Interfaces
{
    public interface IProfileCreatedListener : IAsyncDisposable
    {
        Task InitializeAsync();
        Task ListenAsync();
    }
}