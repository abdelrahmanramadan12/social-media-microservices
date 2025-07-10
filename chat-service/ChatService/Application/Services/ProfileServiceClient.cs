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

        public async Task<Response<List<ProfileDTO>>> GetProfilesAsync(List<string> userIds)
        {
            var result = new Response<List<ProfileDTO>> ();

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/internal/profile/list", new { UserIds = userIds });

                if (!response.IsSuccessStatusCode)
                {
                    result.Success = false;
                    result.Errors = new List<string> { $"HTTP Error: {response.StatusCode}" };
                }

                var responseData = await response.Content.ReadFromJsonAsync<ClientResponse<List<ProfileDTO>>>();

                if (responseData is not null)
                {
                    result.Success = true;
                    result.Value = responseData.Data;
                }
                else
                {
                    result.Success = false;
                    result.Errors = new List<string> { "Null response from profile service." };
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors = new List<string> { ex.Message };
            }

            return result;
        }
    }
}