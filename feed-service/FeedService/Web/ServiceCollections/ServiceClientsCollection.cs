using Application.Abstractions;
using Application.Services;

namespace Web.ServiceCollections
{
    public static class ServiceClientsCollection
    {
        public static IServiceCollection AddServiceClients(this IServiceCollection services)
        {
            services.AddSingleton<IProfileServiceClient, ProfileServiceClient>();
            services.AddSingleton<IFollowServiceClient, FollowServiceClient>();

            return services;
        }
    }
}
