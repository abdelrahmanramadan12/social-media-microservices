using Microsoft.Extensions.Hosting;
using Application.Interfaces.Listeners;

namespace Workers
{
    public class RabbitMqWorker(IFollowListener FollowCreatedListener) : BackgroundService
    {
        private readonly IFollowListener _FollowDeletedListener = FollowCreatedListener;

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _FollowDeletedListener.InitializeAsync();
            await base.StartAsync(cancellationToken);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //var deletedTask = 
            _FollowDeletedListener.ListenAsync(stoppingToken);

            //await Task.WhenAll(deletedTask, deletedTask);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _FollowDeletedListener.DisposeAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}