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

        public async Task<Response<ProfilesListDTO>> GetProfilesAsync(List<string>? follows)
        {
            var result = new Response<ProfilesListDTO>();

            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://your-profile-service.com/api/profile/get-profiles", new { follows });

                if (!response.IsSuccessStatusCode)
                {
                    result.Success = false;
                    result.Errors = new List<string> { $"HTTP Error: {response.StatusCode}" };
                    return result;
                }

                var responseData = await response.Content.ReadFromJsonAsync<ProfilesListDTO>();

                if (responseData is not null)
                {
                    result.Success = true;
                    result.Value = responseData;
                    result.Errors = [];
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