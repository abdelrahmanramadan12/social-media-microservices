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
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private IServiceScope? _scope;
        private IPostEventListner? _postEventConsumer;
        private PostEventPublisher? _postEventPublisher;
        private ReactionEventPublisher? _reactionEventPublisher;

        public QueuePublishersInitializer(
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {

                // Create a scope for resolving scoped services
                _scope = _serviceProvider.CreateScope();
                var scopedProvider = _scope.ServiceProvider;

                // Initialize PostEventConsumer
                _postEventConsumer = scopedProvider.GetRequiredService<IPostEventListner>();
                await _postEventConsumer.InitializeAsync();
                await _postEventConsumer.ListenAsync(stoppingToken);

                // Initialize PostEventPublisher
                _postEventPublisher = new PostEventPublisher( _configuration);
                await _postEventPublisher.InitializeAsync();

                // Initialize ReactionEventPublisher
                _reactionEventPublisher = new ReactionEventPublisher(_configuration);
                await _reactionEventPublisher.InitializeAsync();


                // Keep the service running until cancellation is requested
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {

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
            }
            finally
            {
                _scope?.Dispose();
            }

            await base.StopAsync(cancellationToken);
        }
    }
} 