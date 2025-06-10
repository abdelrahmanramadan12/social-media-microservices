using Microsoft.Extensions.Hosting;
using react_service.Application.Interfaces.Listeners;


namespace Workers
{
    public class QueueListenerWorker : BackgroundService
    {
        private readonly ICommentEventListner _commentEventListener;
        private readonly IPostEventListner _postEventListener;

        public QueueListenerWorker(
            ICommentEventListner commentEventListener, IPostEventListner postEventListener)
        {
            _commentEventListener = commentEventListener;
            _postEventListener = postEventListener;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _commentEventListener.InitializeAsync();
                await _postEventListener.InitializeAsync();
                await base.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting CommentReactionListenerWorker: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task comment = _commentEventListener.ListenAsync(stoppingToken);
            Task post = _postEventListener.ListenAsync(stoppingToken);
            await Task.WhenAll(comment, post);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _commentEventListener.DisposeAsync();
                await _postEventListener.DisposeAsync();
                await base.StopAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping CommentReactionListenerWorker: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
