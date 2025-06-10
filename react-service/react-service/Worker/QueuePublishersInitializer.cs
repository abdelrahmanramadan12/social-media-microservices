using Microsoft.Extensions.Hosting;
using react_service.Application.Events;
using react_service.Application.Interfaces.Publishers;

namespace Worker
{
    public class QueuePublishersInitializer : IHostedService
    {
        private readonly IQueuePublisher<ReactionEvent> _reactionPublisher;
        private readonly IQueuePublisher<CommentReactionEvent> _commentReactionPublisher;
        private readonly IQueuePublisher<ReactionEventNoti> _reactionEventNoti;

        public QueuePublishersInitializer(IQueuePublisher<ReactionEvent> reactionPublisher, 
            IQueuePublisher<CommentReactionEvent> commentReactionPublisher , IQueuePublisher<ReactionEventNoti> reactionEventNoti)
        {
            _reactionPublisher = reactionPublisher;
            _commentReactionPublisher = commentReactionPublisher;
            _reactionEventNoti = reactionEventNoti;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _reactionPublisher.InitializeAsync();
            await _commentReactionPublisher.InitializeAsync();
            await _reactionEventNoti.InitializeAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _reactionPublisher.DisposeAsync();
            await _commentReactionPublisher.DisposeAsync();
            await _reactionEventNoti.DisposeAsync();
        }
    }
}
