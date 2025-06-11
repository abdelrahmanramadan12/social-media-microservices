using Microsoft.Extensions.Hosting;
using Service.Events;
using Service.Interfaces.RabbitMQServices;

namespace Worker
{
    public class RabbitMqWorker: BackgroundService
    {
        private readonly IQueueListener<PostEvent> _postListener;
        private readonly IQueueListener<CommentReactionEvent> _commentReactionListener;
        
        public RabbitMqWorker(IQueueListener<PostEvent> postListener, IQueueListener<CommentReactionEvent> commentReactionListener)
        {
            _postListener = postListener;
            _commentReactionListener = commentReactionListener;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _postListener.InitializeAsync();
            await _commentReactionListener.InitializeAsync();
            await base.StartAsync(cancellationToken);  
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var task = _postListener.ListenAsync(stoppingToken);
            var commentReactionTask = _commentReactionListener.ListenAsync(stoppingToken);
            await Task.WhenAll(task, commentReactionTask);
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _postListener.DisposeAsync();
            await _commentReactionListener.DisposeAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
