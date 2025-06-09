using Application.Abstractions;
using Application.Services;
using Infrastructure.Repositories;

namespace Web.ServiceCollections
{
    public static class FeedServicesCollection
    {
        public static IServiceCollection AddFeedServices(this IServiceCollection services)
        {
            services.AddSingleton<IFeedRepository, FeedRepository>();
            services.AddSingleton<IFeedCommandService, FeedCommandService>();
            services.AddScoped<IFeedQueryService, FeedQueryService>();

            return services;
        }
    }
}
