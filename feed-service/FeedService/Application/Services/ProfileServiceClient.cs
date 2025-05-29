using Application.Abstractions;
using Application.DTOs;
using System.Net.Http.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Services
{
    public class ProfileServiceClient : IProfileServiceClient
    {
        private readonly HttpClient _httpClient;

        public ProfileServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Response<ProfileDTO>> GetProfileAsync(string userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/public/profiles/by-id/{userId}");

                if (!response.IsSuccessStatusCode)
                {
                    return new Response<ProfileDTO>()
                    {
                        Success = false,
                        Value = null,
                        Errors = [$"Profile service returned status code {(int)response.StatusCode}: {response.ReasonPhrase}"]
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ProfileDTO>();

                if (result == null)
                {
                    return new Response<ProfileDTO>()
                    {
                        Success = false,
                        Value = null,
                        Errors = [$"Profile service returned a null or invalid response"]
                    };
                }

                return new Response<ProfileDTO>()
                {
                    Success = true,
                    Value = result,
                    Errors = []
                };
            }
            catch (Exception ex)
            {
                return new Response<ProfileDTO>()
                {
                    Success = false,
                    Value = null,
                    Errors = [$"Unhandled exception: {ex.Message}"]
                };
            }
        }
    }
}