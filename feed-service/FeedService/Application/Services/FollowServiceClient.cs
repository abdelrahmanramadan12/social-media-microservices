using System.Net.Http.Json;
using System.Runtime;
using System.Text;
using System.Text.Json;
using Application.Abstractions;
using Application.DTOs;
using Application.DTOs.Requests;
using Application.DTOs.Responses;

namespace Application.Services
{
    public class FollowServiceClient : IFollowServiceClient
    {
        private readonly HttpClient _httpClient;

        public FollowServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseWrapper<List<string>>> ListFollowersAsync(string userId)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    "/api/internal/follow/list-followers",
                    new ListFollowRequest() { UserId = userId });

                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseWrapper<List<string>>()
                    {
                        Data = null,
                        Errors = [$"Follow service returned status code {response.StatusCode}: {response.ReasonPhrase}"]
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ResponseWrapper<List<string>>>();

                if (result == null || result.Data == null)
                {
                    return new ResponseWrapper<List<string> >()
                    {
                        Data = null,
                        Errors = ["Follow service returned a null or invalid response"]
                    };
                }

                return new ResponseWrapper<List<string> >()
                {
                    Data = result.Data,
                    Errors = []
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<List<string> >()
                {
                    Data = null,
                    Errors = [$"Unhandled exception: {ex.Message}"]
                };
            }
        }
    }
}




