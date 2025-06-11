using Application.Abstractions;
using Application.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Application.Services
{
    public class AuthServiceClient : IAuthServiceClient
    {
        private readonly HttpClient _httpClient;

        public AuthServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Response<AuthResponseDTO>> VerifyTokenAsync(string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/verify");

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return new Response<AuthResponseDTO>()
                    {
                        Success = false,
                        Value = null,
                        Errors = [$"Auth service returned status code {(int)response.StatusCode}: {response.ReasonPhrase}"]
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<AuthResponseDTO>();

                if (result == null)
                {
                    return new Response<AuthResponseDTO>()
                    {
                        Success = false,
                        Value = null,
                        Errors = [$"Auth service returned a null or invalid response"]
                    };
                }

                return new Response<AuthResponseDTO>()
                {
                    Success = true,
                    Value = result,
                    Errors = []
                };
            }
            catch (Exception ex)
            {
                return new Response<AuthResponseDTO>()
                {
                    Success = false,
                    Value = null,
                    Errors = [$"Unhandled exception: {ex.Message}"]
                };
            }
        }
    }
}