using Application.DTOs;
using Application.IServices;
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

        public async Task<ProfilesResponse> GetProfilesAsync(ProfilesRequest profilesRequest)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/public/profiles/by-ids", profilesRequest);

                if (!response.IsSuccessStatusCode)
                {
                    return new ProfilesResponse
                    {
                        Success = false,
                        postAuthorProfiles = new List<PostAuthorProfile>(),
                        Errors = new List<string>
                    {
                        $"Profile service returned status code {(int)response.StatusCode}: {response.ReasonPhrase}"
                    }
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ProfilesResponse>();

                if (result == null)
                {
                    return new ProfilesResponse
                    {
                        Success = false,
                        postAuthorProfiles = new List<PostAuthorProfile>(),
                        Errors = new List<string> { "Profile service returned a null or invalid response." }
                    };
                }

                return result;
            }
            catch (Exception ex)
            {
                return new ProfilesResponse
                {
                    Success = false,
                    postAuthorProfiles = new List<PostAuthorProfile>(),
                    Errors = new List<string> { $"Unhandled exception: {ex.Message}" }
                };
            }
        }
    }
}
