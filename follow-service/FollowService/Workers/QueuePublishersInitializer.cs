using Application.Abstractions;
using Application.Events;
using Microsoft.Extensions.Hosting;

namespace Workers
{
    public class QueuePublishersInitializer : IHostedService
    {
        private readonly IQueuePublisher<FollowEvent> _followPublisher;

        public QueuePublishersInitializer(IQueuePublisher<FollowEvent> followPublisher)
        {
            _followPublisher = followPublisher;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _followPublisher.InitializeAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _followPublisher.DisposeAsync().AsTask();
        }
    }
}
