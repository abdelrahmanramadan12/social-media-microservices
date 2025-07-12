using Application.Interfaces.Listeners;
using Application.Interfaces.Services;
using Application.Services;
using Application.Services.Listeners;
using Domain.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServiceRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ICommentNotificationService, CommentNotificationService>();
            services.AddScoped<IReactionNotificationService, ReactionNotificationService>();
            services.AddScoped<IFollowNotificationService, FollowNotificationService>();
            return services;
        }

        public static IServiceCollection AddRabbitMqListeners(this IServiceCollection services, IConfiguration config)
        {
            var section = config.GetSection("RabbitQueues");

            services.AddSingleton<ICommentListener>(sp =>
            {
                var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                return new CommentListener(
                    Options.Create(CreateListenerSettings(section, "CommentQueue")),
                    scopeFactory
                );
            });

            services.AddSingleton<IReactionListener>(sp =>
            {
                var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                return new ReactionListenerService(
                    Options.Create(CreateListenerSettings(section, "ReactionQueue")),
                    scopeFactory
                );
            });

            services.AddSingleton<IFollowListener>(sp =>
            {
                var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                return new FollowListenerService(
                    Options.Create(CreateListenerSettings(section, "FollowQueue")),
                    scopeFactory
                );
            });

            return services;
        }

        private static RabbitMqListenerSettings CreateListenerSettings(IConfiguration section, string queueKey) =>
            new()
            {
                HostName = section["HostName"],
                Port = int.Parse(section["Port"]),
                UserName = section["Username"],
                Password = section["Password"],
                QueueName = section[queueKey]
            };

    }

}
