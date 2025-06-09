using Microsoft.Extensions.Hosting;
using Service.Interfaces.RabbitMQServices;

namespace Worker
{
    public class RabbitMqWorker: BackgroundService
    {
        private readonly IPostListener _postListener;
        public RabbitMqWorker(IPostListener  postListener )
        {
            _postListener = postListener;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _postListener.InitializeAsync();
            await base.StartAsync(cancellationToken);  
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var task = _postListener.ListenAsync(stoppingToken);
            await Task.WhenAll(task);
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _postListener.DisposeAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
