using Application.Interfaces.Listeners;
using Microsoft.Extensions.Hosting;

namespace Workers
{
    public class RabbitMqWorker(IFollowListener FollowListener,
        ICommentListener commentListener,
        IMessageListener messageNotificationService,
        IReactionListener reactionListener
        ) : BackgroundService
    {
        private readonly IFollowListener _FollowListener = FollowListener;
        private readonly ICommentListener _CommentListener = commentListener;
        private readonly IMessageListener _MessageNotificationService = messageNotificationService;
        private readonly IReactionListener _ReactionListener = reactionListener;

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _FollowListener.InitializeAsync();
            await _CommentListener.InitializeAsync();
            await _MessageNotificationService.InitializeAsync();
            await _ReactionListener.InitializeAsync();
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _FollowListener.ListenAsync(stoppingToken);
            await _CommentListener.ListenAsync(stoppingToken);
            await _MessageNotificationService.ListenAsync(stoppingToken);
            await _ReactionListener.ListenAsync(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _FollowListener.DisposeAsync();
            await _CommentListener.DisposeAsync();
            await _MessageNotificationService.DisposeAsync();
            await _ReactionListener.DisposeAsync();

            await base.StopAsync(cancellationToken);
        }
    }
}