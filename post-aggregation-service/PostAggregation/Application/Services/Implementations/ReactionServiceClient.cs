using Application.Configuration;
using Application.DTOs.Reaction;
using Application.Services.Interfaces;
using System.Net.Http.Json;

namespace Application.Services.Implementations
{
    public class ReactionServiceClient : IReactionServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ReactionServiceSettings _settings;

        public ReactionServiceClient(HttpClient httpClient, ReactionServiceSettings settings)
        {
            _httpClient = httpClient;
            _settings = settings;
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        }

        public async Task<ResponseWrapper<List<string>>> FilterPostsReactedByUserAsync(GetPostsReactedByUserRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/internal/reacts/post/user/filter", request);

                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseWrapper<List<string>>
                    {
                        Errors = new List<string>
                        {
                            $"Reaction service returned status code {(int)response.StatusCode} - {response.ReasonPhrase}"
                        }
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ResponseWrapper<List<string>>>();
                return result ?? new ResponseWrapper<List<string>>
                {
                    Errors = new List<string> { "Response was null" }
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<List<string>>
                {
                    Errors = new List<string> { $"Unhandled exception: {ex.Message}" }
                };
            }
        }

        public async Task<ResponseWrapper<object>> GetPostsReactedByUserAsync(string userId, string? nextReactIdHash = null)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/internal/reacts/post/user/{userId}?nextReactIdHash={nextReactIdHash ?? ""}");

                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseWrapper<object>
                    {
                        Errors = new List<string>
                        {
                            $"Reaction service returned status code {(int)response.StatusCode} - {response.ReasonPhrase}"
                        }
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ResponseWrapper<object>>();
                return result ?? new ResponseWrapper<object>
                {
                    Errors = new List<string> { "Response was null" }
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<object>
                {
                    Errors = new List<string> { $"Unhandled exception: {ex.Message}" }
                };
            }
        }

        public async Task<ResponseWrapper<object>> GetReactsOfPostAsync(GetReactsOfPostRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/internal/reacts/post", request);

                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseWrapper<object>
                    {
                        Errors = new List<string>
                        {
                            $"Reaction service returned status code {(int)response.StatusCode} - {response.ReasonPhrase}"
                        }
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ResponseWrapper<object>>();
                return result ?? new ResponseWrapper<object>
                {
                    Errors = new List<string> { "Response was null" }
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<object>
                {
                    Errors = new List<string> { $"Unhandled exception: {ex.Message}" }
                };
            }
        }
    }
}

