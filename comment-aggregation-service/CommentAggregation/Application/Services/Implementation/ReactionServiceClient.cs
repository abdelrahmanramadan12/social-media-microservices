using Application.DTOs;
using Application.DTOs.Reaction;
using Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Application.Services.Implementation
{
    public class ReactionServiceClient : IReactionServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ReactionServiceClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["Clients:Reaction"] ?? throw new ArgumentNullException("Clients:Reaction");
        }

        public async Task<ResponseWrapper<List<string>>> FilterCommentsReactedByUserAsync(FilterCommentsReactedByUserRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_baseUrl.TrimEnd('/') + "/api/internal/reacts/comment/filter", request);
                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseWrapper<List<string>>
                    {
                        Errors = new List<string> { $"Failed to filter comments: {response.StatusCode}" },
                        ErrorType = ErrorType.InternalServerError
                    };
                }
                var result = await response.Content.ReadFromJsonAsync<ResponseWrapper<List<string>>>();
                return result ?? new ResponseWrapper<List<string>>
                {
                    Errors = new List<string> { "Empty response from reaction service" },
                    ErrorType = ErrorType.InternalServerError
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<List<string>>
                {
                    Errors = new List<string> { $"Error filtering comments: {ex.Message}" },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }
    }
}
