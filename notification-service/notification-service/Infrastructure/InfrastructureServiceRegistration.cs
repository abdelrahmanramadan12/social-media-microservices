using Application.Interfaces;
using Domain.CacheEntities;
using Domain.CacheEntities.Comments;
using Domain.CacheEntities.Reactions;
using Domain.CoreEntities;
using Infrastructure.Repositories;
using Infrastructure.SeedingData.CacheSeeding;
using Infrastructure.SeedingData.mongdbSeeding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<RedisFollowsSeeder>();
            services.AddScoped<RedisReactionsSeeder>();
            services.AddScoped<RedisCommentsSeeder>();


            services.AddScoped<MongoFollowsSeeder>();
            services.AddScoped<MongoReactionsSeeder>();
            services.AddScoped<MongoCommentsSeeder> ();



            return services;
        }
    }



}
