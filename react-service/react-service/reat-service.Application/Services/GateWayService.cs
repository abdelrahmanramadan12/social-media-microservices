using reat_service.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Http;
using System.Net.Http.Json;
using reat_service.Application.DTO.ReactionPost.Response;




namespace reat_service.Infrastructure.Services
{
        public class GatewayService : IGatewayService
        {
            private readonly HttpClient _httpClient;

            public GatewayService(IHttpClientFactory httpClientFactory)
            {
                _httpClient = httpClientFactory.CreateClient("GatewayClient");
            }

            public async Task<string?> GetServiceUrlAsync(string serviceName)
            {
                try
                {
                    var response = await _httpClient.GetFromJsonAsync<ServiceUrlResponse>($"/service/{serviceName}");
                    return response?.Url;
                }
                catch
                {
                    return null;
                }
            }
            public async Task<T?> CallServiceAsync<T>(string serviceName, string endpoint)
            {
                var baseUrl = await GetServiceUrlAsync(serviceName);
                if (string.IsNullOrEmpty(baseUrl))
                    throw new Exception($"Service '{serviceName}' is not available.");

                try
                {
                    var response = await _httpClient.GetFromJsonAsync<T>($"{baseUrl}{endpoint}");
                    return response;
                }
                catch (Exception ex)
                {
                    // You could log this exception or wrap it in a custom exception type
                    throw new Exception($"Failed to call {serviceName} at {endpoint}", ex);
                }
            }


    }
}

