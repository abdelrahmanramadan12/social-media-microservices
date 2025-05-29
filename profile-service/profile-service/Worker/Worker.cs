using Microsoft.Extensions.Hosting;
using Service.Interfaces.RabbitMqServices;

namespace Worker
{
    public class RabbitMqWorker : BackgroundService
    {
        private readonly IFollowListener _followListener;
        public RabbitMqWorker(IFollowListener followListener)
        {
            _followListener = followListener;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _followListener.InitializeAsync();
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var task = _followListener.ListenAsync(stoppingToken);
            await Task.WhenAll(task);
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _followListener.DisposeAsync();
            await base.StopAsync(cancellationToken);

        }
    }
}