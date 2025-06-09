using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using react_service.Infrastructure.Mongodb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using react_service.Application.Interfaces.Services;
using react_service.Application.Services;
using react_service.Infrastructure.Repositories;
using react_service.Application.Interfaces.Repositories;
using Microsoft.Extensions.Options;
using react_service.Application.Pagination;

namespace react_service.Infrastructure
{

    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IPostReactionRepository>(sp =>
            {
                var mongoDbSettings = sp.GetRequiredService<IOptions<MongoDbSettings>>();
                var mongoClient = sp.GetRequiredService<IMongoClient>();
                var paginationSettings = sp.GetRequiredService<IOptions<PaginationSettings>>();
                return new PostReactionRepositoy(mongoDbSettings, mongoClient, paginationSettings);
            });
            services.AddScoped<ICommentReactionRepository>(sp =>
            {
                var mongoDbSettings = sp.GetRequiredService<IOptions<MongoDbSettings>>();
                var mongoClient = sp.GetRequiredService<IMongoClient>();
                var paginationSettings = sp.GetRequiredService<IOptions<PaginationSettings>>();
                return new CommentReactionRepository(mongoDbSettings, mongoClient, paginationSettings);
            });
            services.AddScoped<ICommentRepository>(sp =>
            {
                var mongoDbSettings = sp.GetRequiredService<IOptions<MongoDbSettings>>();
                var mongoClient = sp.GetRequiredService<IMongoClient>();
                return new CommentRepository(mongoDbSettings, mongoClient);
            });
            return services;
        }
    }
}
