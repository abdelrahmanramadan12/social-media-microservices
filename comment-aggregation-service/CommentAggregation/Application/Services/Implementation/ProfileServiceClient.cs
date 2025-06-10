using Application.DTOs;
using Application.DTOs.Profile;
using Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implementation
{
    public class ProfileServiceClient : IProfileServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private const string BASE_ENDPOINT = "/api/internal/profile";

        public ProfileServiceClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["Clients:Profile"] ?? throw new ArgumentNullException("Clients:Profile");
        }

        public async Task<ResponseWrapper<List<SimpleUserProfile>>> GetUsersByIdsAsync(GetUsersProfileByIdsRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_baseUrl.TrimEnd('/') + $"{BASE_ENDPOINT}/list", request);
                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseWrapper<List<SimpleUserProfile>>
                    {
                        Errors = new List<string> { $"HTTP Error: {response.StatusCode}" },
                        Message = "Failed to retrieve user profiles."
                    };
                }
                var responseData = await response.Content.ReadFromJsonAsync<ResponseWrapper<List<SimpleUserProfile>>>();
                return responseData ?? new ResponseWrapper<List<SimpleUserProfile>>
                {
                    Errors = new List<string> { "Null response from profile service." },
                    Message = "Failed to retrieve user profiles."
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<List<SimpleUserProfile>>
                {
                    Errors = new List<string> { ex.Message },
                    Message = "Exception occurred while retrieving user profiles."
                };
            }
        }
    }
}
