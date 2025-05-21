using Microsoft.Extensions.Hosting;
using Service.Interfaces;

namespace Worker
{
    public class RabbitMqWorker: BackgroundService
    {
        private readonly IPostDeletedListener _postDeletedListener;
        public RabbitMqWorker(IPostDeletedListener  postDeletedListener )
        {
            _postDeletedListener = postDeletedListener;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _postDeletedListener.InitializeAsync();
            await base.StartAsync(cancellationToken);  
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var task = _postDeletedListener.ListenAsync(stoppingToken);
            await Task.WhenAll(task);
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _postDeletedListener.DisposeAsync();
            await base.StopAsync(cancellationToken);

        }
    }
}
