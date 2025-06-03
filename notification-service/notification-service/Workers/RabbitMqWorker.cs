using Microsoft.Extensions.Hosting;
using Application.Interfaces.Listeners;

namespace Workers
{
    public class RabbitMqWorker(IReactionListener ReactionCreatedListener) : BackgroundService
    {
        private readonly IReactionListener _ReactionDeletedListener = ReactionCreatedListener;

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _ReactionDeletedListener.InitializeAsync();
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var deletedTask = _ReactionDeletedListener.ListenAsync(stoppingToken);

            await Task.WhenAll(deletedTask, deletedTask);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _ReactionDeletedListener.DisposeAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}