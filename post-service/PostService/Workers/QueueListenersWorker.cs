using Application.Events;
using Application.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Workers
{
    public class QueueListenersWorker : BackgroundService
    {
        private readonly IQueueListener<CommentEvent> _commentEventConsumer;
        private readonly IQueueListener<ReactionEvent> _reactionEventConsumer;

        public QueueListenersWorker(IQueueListener<CommentEvent> commentListener, IQueueListener<ReactionEvent> reactionListener)
        {
            _commentEventConsumer = commentListener;
            _reactionEventConsumer = reactionListener;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _commentEventConsumer.InitializeAsync();
            await _reactionEventConsumer.InitializeAsync();
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task commentTask = _commentEventConsumer.ListenAsync(stoppingToken);
            Task reactionTask = _reactionEventConsumer.ListenAsync(stoppingToken);
            await Task.WhenAll(commentTask, reactionTask);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_commentEventConsumer != null)
                await _commentEventConsumer.DisposeAsync();
            if (_reactionEventConsumer != null)
                await _reactionEventConsumer.DisposeAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
