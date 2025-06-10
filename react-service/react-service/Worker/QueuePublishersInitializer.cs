using Microsoft.Extensions.Hosting;
using react_service.Application.Events;
using react_service.Application.Interfaces.Publishers;

namespace Worker
{
    public class QueuePublishersInitializer : IHostedService
    {
        private readonly IQueuePublisher<ReactionEvent> _reactionPublisher;
        private readonly IQueuePublisher<CommentReactionEvent> _commentReactionPublisher;

        public QueuePublishersInitializer(IQueuePublisher<ReactionEvent> reactionPublisher, IQueuePublisher<CommentReactionEvent> commentReactionPublisher)
        {
            _reactionPublisher = reactionPublisher;
            _commentReactionPublisher = commentReactionPublisher;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _reactionPublisher.InitializeAsync();
            await _commentReactionPublisher.InitializeAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _reactionPublisher.DisposeAsync();
            await _commentReactionPublisher.DisposeAsync();
        }
    }
}
