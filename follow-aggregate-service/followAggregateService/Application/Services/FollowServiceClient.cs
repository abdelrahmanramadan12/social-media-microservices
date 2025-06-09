using Application.Abstractions;
using Application.DTOs;
using System.Net.Http.Json;

namespace Application.Services
{
    public class FollowServiceClient : IFollowServiceClient
    {
        private readonly HttpClient _httpClient;

        public FollowServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Response<FollowsPageDTO>> ListFollowersPage(string userId, string? cursor)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/internal/follows/list-followers-page/{userId}?next={cursor}");

                if (!response.IsSuccessStatusCode)
                {
                    return new Response<FollowsPageDTO>()
                    {
                        Success = false,
                        Value = null,
                        Errors = [$"Follow service returned status code {(int)response.StatusCode}: {response.ReasonPhrase}"]
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<FollowsPageDTO>();

                if (result == null)
                {
                    return new Response<FollowsPageDTO>()
                    {
                        Success = false,
                        Value = null,
                        Errors = [$"Follow service returned a null or invalid response"]
                    };
                }

                return new Response<FollowsPageDTO>()
                {
                    Success = true,
                    Value = result,
                    Errors = []
                };
            }
            catch (Exception ex)
            {
                return new Response<FollowsPageDTO>()
                {
                    Success = false,
                    Value = null,
                    Errors = [$"Unhandled exception: {ex.Message}"]
                };
            }
        }

        public async Task<Response<FollowsPageDTO>> ListFollowingPage(string userId, string? cursor)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/internal/follows/list-following-page/{userId}?next={cursor}");

                if (!response.IsSuccessStatusCode)
                {
                    return new Response<FollowsPageDTO>()
                    {
                        Success = false,
                        Value = null,
                        Errors = [$"Follow service returned status code {(int)response.StatusCode}: {response.ReasonPhrase}"]
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<FollowsPageDTO>();

                if (result == null)
                {
                    return new Response<FollowsPageDTO>()
                    {
                        Success = false,
                        Value = null,
                        Errors = [$"Follow service returned a null or invalid response"]
                    };
                }

                return new Response<FollowsPageDTO>()
                {
                    Success = true,
                    Value = result,
                    Errors = []
                };
            }
            catch (Exception ex)
            {
                return new Response<FollowsPageDTO>()
                {
                    Success = false,
                    Value = null,
                    Errors = [$"Unhandled exception: {ex.Message}"]
                };
            }
        }
    }
}




