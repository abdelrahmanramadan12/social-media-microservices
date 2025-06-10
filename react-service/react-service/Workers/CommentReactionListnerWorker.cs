using react_service.Application.Interfaces.Listeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Workers
{
    public class CommentReactionListnerWorker : BackgroundService
    {
        private readonly ICommentEventListner _commentEventListner;
        private readonly CommentEventListner _commentEventConsumer;

        public CommentReactionListnerWorker(
            ICommentEventListner commentEventListener,
            CommentEventListner commentEventConsumer)
        {
            _commentEventListner = commentEventListener;
            _commentEventConsumer = commentEventConsumer;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _commentEventListner.InitializeAsync();
                await _commentEventConsumer.InitializeAsync();
                await base.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting CommentReactionListnerWorker: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var deletedTask = _commentEventListner.ListenAsync(stoppingToken);
                var commentEventTask = _commentEventConsumer.ListenAsync(stoppingToken);

                // Create a task that will complete when the stoppingToken is cancelled
                var cancellationTask = Task.Delay(Timeout.Infinite, stoppingToken);

                // Wait for any of the tasks to complete
                await Task.WhenAny(deletedTask, commentEventTask, cancellationTask);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CommentReactionListnerWorker ExecuteAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _commentEventListner.DisposeAsync();
                await _commentEventConsumer.DisposeAsync();
                await base.StopAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping CommentReactionListnerWorker: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
