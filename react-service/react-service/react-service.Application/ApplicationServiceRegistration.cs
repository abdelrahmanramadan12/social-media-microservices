using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using react_service.Application.Interfaces.Repositories;
using react_service.Application.Interfaces.Services;
using react_service.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Application
{
   public  static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServiceRegistration(this IServiceCollection services ,IConfiguration configuration)
        {
           


            services.AddScoped<IReactionPostService, ReactionPostService>();
            return services;
        }
    }
}
