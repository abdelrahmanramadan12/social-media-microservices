using Domain.Events;

namespace Application.Interfaces.Listeners
{
    public interface IMessageListener :IAsyncDisposable
    {

        Task InitializeAsync();
        Task ListenAsync(CancellationToken cancellationToken);
    }
}
