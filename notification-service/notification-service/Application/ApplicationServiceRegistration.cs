using Application.Interfaces.Listeners;
using Application.Interfaces.Services;
using Application.Services;
using Application.Services.Listeners;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
    }

}
