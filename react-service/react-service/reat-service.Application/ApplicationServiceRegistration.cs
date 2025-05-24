using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using reat_service.Application.Interfaces.Repositories;
using reat_service.Application.Interfaces.Services;
using reat_service.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reat_service.Application
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
