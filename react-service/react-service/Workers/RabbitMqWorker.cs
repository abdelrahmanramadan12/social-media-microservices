using Microsoft.Extensions.Hosting;
using react_service.Application.Interfaces.Listeners;

namespace Workers
{
    public class RabbitMqWorker : BackgroundService
    {
        private readonly IPostDeletedListener _postDeletedListener;

        public RabbitMqWorker(IPostDeletedListener PostCreatedListener)
        {
            _postDeletedListener = PostCreatedListener;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _postDeletedListener.InitializeAsync();
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var deletedTask = _postDeletedListener.ListenAsync(stoppingToken);

            await Task.WhenAll(deletedTask, deletedTask);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _postDeletedListener.DisposeAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}