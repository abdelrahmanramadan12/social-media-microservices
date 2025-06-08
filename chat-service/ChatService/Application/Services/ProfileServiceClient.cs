using Application.Abstractions;
using Application.DTOs;
using System.Net.Http.Json;

namespace Application.Services
{
    public class ProfileServiceClient : IProfileServiceClient
    {
        private readonly HttpClient _httpClient;

        public ProfileServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseWrapper<List<ProfileDTO>>> GetProfilesAsync(List<string> userIds)
        {
            var result = new ResponseWrapper<List<ProfileDTO>> ();

            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7221/api/internal/profile/list/", new { UserIds = userIds });

                if (!response.IsSuccessStatusCode)
                {
                    result.Errors = new List<string> { $"HTTP Error: {response.StatusCode}" };
                    return result;
                }

                var responseData = await response.Content.ReadFromJsonAsync<ResponseWrapper<List<ProfileDTO>>>();

                if (responseData is not null)
                {
                    result = responseData;
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