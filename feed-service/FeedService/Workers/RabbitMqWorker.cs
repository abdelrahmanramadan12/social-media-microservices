using Application.Abstractions;
using Application.Events;
using Microsoft.Extensions.Hosting;

namespace Workers
{
    public class RabbitMqWorker : BackgroundService
    {
        private readonly IQueueListener<PostEvent> _postListener;
        private readonly IQueueListener<ProfileEvent> _profileListener;
        private readonly IQueueListener<FollowEvent> _followListener;
        private readonly IQueueListener<CommentEvent> _commentListener;
        private readonly IQueueListener<ReactEvent> _reactListener;

        public RabbitMqWorker (
            IQueueListener<PostEvent> postListener,
            IQueueListener<ProfileEvent> profileListener,
            IQueueListener<FollowEvent> followListener,
            IQueueListener<CommentEvent> commentListener,
            IQueueListener<ReactEvent> reactListener
        )
        {
            _postListener = postListener;
            _profileListener = profileListener;
            _followListener = followListener;
            _commentListener = commentListener;
            _reactListener = reactListener;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _postListener.InitializeAsync();
            await _profileListener.InitializeAsync();
            await _followListener.InitializeAsync();
            await _commentListener.InitializeAsync();
            await _reactListener.InitializeAsync();
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var postTask = _postListener.ListenAsync(stoppingToken);
            var profileTask = _profileListener.ListenAsync(stoppingToken);
            var followTask = _followListener.ListenAsync(stoppingToken);
            var commentTask = _commentListener.ListenAsync(stoppingToken);
            var reactTask = _reactListener.ListenAsync(stoppingToken);

            await Task.WhenAll(postTask, profileTask, followTask, commentTask, reactTask);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _postListener.DisposeAsync();
            await _profileListener.DisposeAsync();
            await _followListener.DisposeAsync();
            await _commentListener.DisposeAsync();
            await _reactListener.DisposeAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
