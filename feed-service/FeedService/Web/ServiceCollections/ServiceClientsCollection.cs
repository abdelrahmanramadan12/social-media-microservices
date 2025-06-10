using Application.Abstractions;
using Application.Services;

namespace Web.ServiceCollections
{
    public static class ServiceClientsCollection
    {
        public static IServiceCollection AddServiceClients(this IServiceCollection services)
        {
            services.AddHttpClient<IProfileServiceClient, ProfileServiceClient>();
            services.AddHttpClient<IFollowServiceClient, FollowServiceClient>();

            return services;
        }
    }
}
