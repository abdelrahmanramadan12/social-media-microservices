
using Microsoft.Extensions.Hosting;
using react_service.Domain.Events;
using react_service.Domain.interfaces;

namespace Workers
{
    public class ReactionPublisherWorker : IHostedService
    {
        private readonly IQueuePublisher<ReactionEvent> _reactionPublisher;

        public ReactionPublisherWorker(IQueuePublisher<ReactionEvent> followPublisher)
        {
            _reactionPublisher = followPublisher;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _reactionPublisher.InitializeAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _reactionPublisher.DisposeAsync().AsTask();
        }

     
    }

  
}