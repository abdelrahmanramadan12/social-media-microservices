using Application.Events;
using Application.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Workers
{
    public class QueuePublishersInitializer : IHostedService
    {
        private readonly IQueuePublisher<PostEvent> _postPublisher;

        public QueuePublishersInitializer(IQueuePublisher<PostEvent> postPublisher)
        {
            _postPublisher = postPublisher;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _postPublisher.InitializeAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _postPublisher.DisposeAsync();
        }
    }
}
