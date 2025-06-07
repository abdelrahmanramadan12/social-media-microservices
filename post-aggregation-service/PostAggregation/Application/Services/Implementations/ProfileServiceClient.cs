using Application.Configuration;
using Application.DTOs;
using Application.DTOs.Profile;
using Application.Services.Interfaces;
using System.Net.Http.Json;

namespace Application.Services.Implementations
{
    public class ProfileServiceClient : IProfileServiceClient
    {
        private readonly HttpClient _httpClient;
        private const string BASE_ENDPOINT = "api/internal/profile";
        private readonly ProfileServiceSettings _settings;

        public ProfileServiceClient(HttpClient httpClient, ProfileServiceSettings settings)
        {
            _httpClient = httpClient;
            _settings = settings;
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        }

        public async Task<ResponseWrapper<SimpleUserProfile>> GetByUserIdMinAsync(string userId)
        {
            var result = new ResponseWrapper<SimpleUserProfile>();

            try
            {
                var response = await _httpClient.GetAsync($"{BASE_ENDPOINT}/min/id/{userId}");

                if (!response.IsSuccessStatusCode)
                {
                    result.Errors = new List<string> { $"HTTP Error: {response.StatusCode}" };
                    result.Message = "Failed to retrieve user profile.";
                    return result;
                }

                var responseData = await response.Content.ReadFromJsonAsync<ResponseWrapper<SimpleUserProfile>>();

                if (responseData is not null)
                {
                    result.Data = responseData.Data;
                    result.Errors = responseData.Errors ?? new List<string>();
                    result.Message = responseData.Message ?? "User profile retrieved successfully.";
                }
                else
                {
                    result.Errors = new List<string> { "Null response from profile service." };
                }
            }
            catch (Exception ex)
            {
                result.Errors = new List<string> { ex.Message };
            }

            return result;
        }

        public async Task<ResponseWrapper<List<SimpleUserProfile>>> GetUsersByIdsAsync(GetUsersProfileByIdsRequest request)
        {
            var result = new ResponseWrapper<List<SimpleUserProfile>>();

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BASE_ENDPOINT}/list", request);

                if (!response.IsSuccessStatusCode)
                {
                    result.Errors = new List<string> { $"HTTP Error: {response.StatusCode}" };
                    result.Message = "Failed to retrieve user profiles.";
                    return result;
                }

                var responseData = await response.Content.ReadFromJsonAsync<ResponseWrapper<List<SimpleUserProfile>>>();

                if (responseData is not null)
                {
                    result.Data = responseData.Data;
                    result.Errors = responseData.Errors ?? new List<string>();
                    result.Message = responseData.Message ?? "User profiles retrieved successfully.";
                }
                else
                {
                    result.Errors = new List<string> { "Null response from profile service." };
                }
            }
            catch (Exception ex)
            {
                result.Errors = new List<string> { ex.Message };
            }

            return result;
        }
    }
}
