using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{


        public static class ApplicationServiceRegistration
        {
            public static IServiceCollection AddApplicationServiceRegistration(this IServiceCollection services, IConfiguration configuration)
            {



                return services;
            }
        }
    
}
