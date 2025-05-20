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
            await _profileCreatedListener.ListenAsync();
            await _profileDeletedListener.ListenAsync();

            if (stoppingToken.IsCancellationRequested)
            {
                await _profileCreatedListener.DisposeAsync();
                await _profileDeletedListener.DisposeAsync();
            }
        }
    }
}
