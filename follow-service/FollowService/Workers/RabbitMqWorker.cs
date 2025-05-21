using Microsoft.Extensions.Hosting;
using Services.Interfaces;

namespace Workers
{
    public class RabbitMqWorker : BackgroundService
    {
        private readonly IProfileCreatedListener _profileCreatedListener;
        private readonly IProfileDeletedListener _profileDeletedListener;

        public RabbitMqWorker(IProfileCreatedListener profileCreatedListener, IProfileDeletedListener profileDeletedListener)
        {
            _profileCreatedListener = profileCreatedListener;
            _profileDeletedListener = profileDeletedListener;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _profileCreatedListener.InitializeAsync();
            await _profileDeletedListener.InitializeAsync();
            await base.StartAsync(cancellationToken); 
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var createdTask = _profileCreatedListener.ListenAsync(stoppingToken);
            var deletedTask = _profileDeletedListener.ListenAsync(stoppingToken);

            await Task.WhenAll(createdTask, deletedTask);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _profileCreatedListener.DisposeAsync();
            await _profileDeletedListener.DisposeAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
