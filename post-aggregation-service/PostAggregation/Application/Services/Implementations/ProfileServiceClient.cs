using Application.DTOs;
using Application.DTOs.Profile;
using Application.Services.Interfaces;
using System.Net.Http.Json;

namespace Application.Services.Implementations
{
    public class ProfileServiceClient : IProfileServiceClient
    {
        private readonly HttpClient _httpClient;

        public ProfileServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResponseDTO<PostAuthorProfile>> GetProfileResponse(SingleProfileRequest request)
        {
            var result = new ServiceResponseDTO<PostAuthorProfile>();

            try
            {
                // Replace with your actual endpoint
                var response = await _httpClient.PostAsJsonAsync("https://your-profile-service.com/api/profile/get-profile", request);

                if (!response.IsSuccessStatusCode)
                {
                    result.Success = false;
                    result.Errors = new List<string> { $"HTTP Error: {response.StatusCode}" };
                    return result;
                }

                var responseData = await response.Content.ReadFromJsonAsync<SingleProfileResponse>();

                if (responseData is not null)
                {
                    result.Success = responseData.Success;
                    result.Item = responseData.PostAuthorProfile;
                    result.Errors = responseData.Errors ?? new List<string>();
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
