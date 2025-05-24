using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reat_service.Application.Interfaces.Services
{
    public interface  IGatewayService
    {
        Task<string> GetServiceUrlAsync(string serviceName);
         Task<T?> CallServiceAsync<T>(string serviceName, string endpoint);


    }
}
