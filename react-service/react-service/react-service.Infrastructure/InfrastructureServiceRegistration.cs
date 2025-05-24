using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using react_service.Infrastructure.Mongodb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using reat_service.Application.Interfaces.Services;
using reat_service.Application.Services;
using react_service.Infrastructure.Repositories;
using reat_service.Application.Interfaces.Repositories;

namespace react_service.Infrastructure
{
   
        public static class InfrastructureServiceRegistration
        {
            public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
            {


               services.AddScoped<IReactionPostRepository, ReactionPostRepository>();

                return services;
            }
        }

   
}
