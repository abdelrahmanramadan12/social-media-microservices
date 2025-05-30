using Domain.CacheEntities;
using Domain.CacheEntities.Comments;
using Domain.CacheEntities.Reactions;
using Domain.CoreEntities;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IGenericRepository<Comment>, CoreGenericRepository<Comment>>();
            services.AddScoped<IGenericRepository<Follows>, CoreGenericRepository<Follows>>();
            services.AddScoped<IGenericRepository<Messages>, CoreGenericRepository<Messages>>();
            services.AddScoped<IGenericRepository<Reaction>, CoreGenericRepository<Reaction>>();
            services.AddScoped<IGenericRepository<CachedComments>, CacheGenericRepository<CachedComments>>();
            services.AddScoped<IGenericRepository<CachedReactions>, CacheGenericRepository<CachedReactions>>();
            services.AddScoped<IGenericRepository<CachedFollowed>, CacheGenericRepository<CachedFollowed>>();
            return services;
        }
    }



}
