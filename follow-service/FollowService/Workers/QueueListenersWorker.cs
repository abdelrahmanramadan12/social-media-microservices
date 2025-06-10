using Microsoft.Extensions.Hosting;
using Application.Abstractions;
using Application.Events;

namespace Workers
{
    public class QueueListenersWorker : BackgroundService
    {
        private readonly IQueueListener<ProfileEvent> _profileListener;

        public QueueListenersWorker(IQueueListener<ProfileEvent> profileListener)
        {
            _profileListener = profileListener;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _profileListener.InitializeAsync();
            await base.StartAsync(cancellationToken); 
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _profileListener.ListenAsync(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _profileListener.DisposeAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
