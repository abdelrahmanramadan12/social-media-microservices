using Microsoft.Extensions.Hosting;
using react_service.Application.Interfaces.Listeners;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Workers
{
    public class RabbitMqWorker : BackgroundService
    {
        private readonly IPostEventListner _postEventListner;
        private readonly PostEventListner _postEventConsumer;

        public RabbitMqWorker(
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
                Console.WriteLine("Starting RabbitMqWorker...");
                
                Console.WriteLine("Initializing PostDeletedListener...");
                await _postEventListner.InitializeAsync();
                Console.WriteLine("PostDeletedListener initialized successfully.");
                
                Console.WriteLine("Initializing PostEventListner...");
                await _postEventConsumer.InitializeAsync();
                Console.WriteLine("PostEventListner initialized successfully.");
                
                await base.StartAsync(cancellationToken);
                Console.WriteLine("RabbitMqWorker started successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting RabbitMqWorker: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Console.WriteLine("Starting RabbitMQ consumers...");
                
                var deletedTask = _postEventListner.ListenAsync(stoppingToken);
                var postEventTask = _postEventConsumer.ListenAsync(stoppingToken);

                // Create a task that will complete when the stoppingToken is cancelled
                var cancellationTask = Task.Delay(Timeout.Infinite, stoppingToken);

                Console.WriteLine("Waiting for tasks to complete...");
                // Wait for any of the tasks to complete
                await Task.WhenAny(deletedTask, postEventTask, cancellationTask);
                
                Console.WriteLine("One of the tasks completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RabbitMqWorker ExecuteAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine("Stopping RabbitMqWorker...");
                
                Console.WriteLine("Disposing PostDeletedListener...");
                await _postEventListner.DisposeAsync();
                
                Console.WriteLine("Disposing PostEventListner...");
                await _postEventConsumer.DisposeAsync();
                
                await base.StopAsync(cancellationToken);
                Console.WriteLine("RabbitMqWorker stopped successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping RabbitMqWorker: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}