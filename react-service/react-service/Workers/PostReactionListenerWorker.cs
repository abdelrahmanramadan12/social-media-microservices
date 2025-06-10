using Microsoft.Extensions.Hosting;
using react_service.Application.Interfaces.Listeners;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Workers
{
    public class PostReactionListnerWorker : BackgroundService
    {
        private readonly IPostEventListner _postEventListner;
        private readonly PostEventListner _postEventConsumer;

        public PostReactionListnerWorker(
            IPostEventListner postEventListener,
            PostEventListner postEventConsumer)
        {
            _postEventListner = postEventListener;
            _postEventConsumer = postEventConsumer;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _postEventListner.InitializeAsync();
                await _postEventConsumer.InitializeAsync();
                await base.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting PostReactionListnerWorker: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var deletedTask = _postEventListner.ListenAsync(stoppingToken);
                var postEventTask = _postEventConsumer.ListenAsync(stoppingToken);

                // Create a task that will complete when the stoppingToken is cancelled
                var cancellationTask = Task.Delay(Timeout.Infinite, stoppingToken);

                // Wait for any of the tasks to complete
                await Task.WhenAny(deletedTask, postEventTask, cancellationTask);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PostReactionListnerWorker ExecuteAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _postEventListner.DisposeAsync();
                await _postEventConsumer.DisposeAsync();
                await base.StopAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping PostReactionListnerWorker: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}