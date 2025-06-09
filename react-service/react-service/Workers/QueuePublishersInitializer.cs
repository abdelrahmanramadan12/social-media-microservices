using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using react_service.Application.Interfaces.Listeners;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Workers
{
    public class QueuePublishersInitializer : BackgroundService
    {
        private readonly ILogger<QueuePublishersInitializer> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private IServiceScope? _scope;
        private IPostEventListner? _postEventConsumer;
        private PostEventPublisher? _postEventPublisher;
        private ReactionEventPublisher? _reactionEventPublisher;

        public QueuePublishersInitializer(
            ILogger<QueuePublishersInitializer> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Starting QueuePublishInitializer...");

                // Create a scope for resolving scoped services
                _scope = _serviceProvider.CreateScope();
                var scopedProvider = _scope.ServiceProvider;

                // Initialize PostEventConsumer
                _postEventConsumer = scopedProvider.GetRequiredService<IPostEventListner>();
                await _postEventConsumer.InitializeAsync();
                await _postEventConsumer.ListenAsync(stoppingToken);

                // Initialize PostEventPublisher
                _postEventPublisher = new PostEventPublisher(
                    scopedProvider.GetRequiredService<ILogger<PostEventPublisher>>(),
                    _configuration);
                await _postEventPublisher.InitializeAsync();

                // Initialize ReactionEventPublisher
                _reactionEventPublisher = new ReactionEventPublisher(
                    scopedProvider.GetRequiredService<ILogger<ReactionEventPublisher>>(),
                    _configuration);
                await _reactionEventPublisher.InitializeAsync();

                _logger.LogInformation("All queue services initialized successfully");

                // Keep the service running until cancellation is requested
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in QueuePublishInitializer");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping QueuePublishInitializer...");

            try
            {
                if (_postEventConsumer != null)
                {
                    await _postEventConsumer.DisposeAsync();
                }
                if (_postEventPublisher != null)
                {
                    await _postEventPublisher.DisposeAsync();
                }
                if (_reactionEventPublisher != null)
                {
                    await _reactionEventPublisher.DisposeAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing queue services");
            }
            finally
            {
                _scope?.Dispose();
            }

            await base.StopAsync(cancellationToken);
        }
    }
} 