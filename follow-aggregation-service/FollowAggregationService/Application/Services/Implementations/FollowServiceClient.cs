using Application.Configurations;
using Application.DTOs;
using Application.DTOs.Follow;
using Application.Services.Interfaces;

namespace Application.Services.Implementations
{
    public class FollowServiceClient : IFollowServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly FollowServiceSettings _settings;

        public FollowServiceClient(HttpClient httpClient, FollowServiceSettings settings)
        {
            _httpClient = httpClient;
            _settings = settings;
        }
        public Task<PaginationResponseWrapper<List<string>>> GetFollowers(ListFollowPageRequest request)
        {
            return PostRequestAsync("/api/internal/follow/list-followers-page", request, "Failed to fetch followers");
        }

        public Task<PaginationResponseWrapper<List<string>>> GetFollowing(ListFollowPageRequest request)
        {
            return PostRequestAsync("/api/internal/follow/list-following-page", request, "Failed to fetch following");
        }

        private async Task<PaginationResponseWrapper<List<string>>> PostRequestAsync(string endpoint, ListFollowPageRequest request, string failureMessage)
        {
            try
            {
                var url = $"{_settings.BaseUrl}{endpoint}";
                var requestContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, requestContent);

                if (!response.IsSuccessStatusCode)
                {
                    try
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var result = System.Text.Json.JsonSerializer.Deserialize<PaginationResponseWrapper<List<string>>>(json);

                        return result ?? new PaginationResponseWrapper<List<string>>
                        {
                            Data = new List<string>(),
                            Errors = new List<string> { "Failed to load data from the follow service." },
                            ErrorType = ErrorType.InternalServerError
                        };
                    }
                    catch
                    {
                        return new PaginationResponseWrapper<List<string>>
                        {
                            Data = new List<string>(),
                            Errors = new List<string> { "Failed to load data from the follow service." },
                            ErrorType = ErrorType.InternalServerError
                        };
                    }
                }


                return new PaginationResponseWrapper<List<string>>
                {
                    Data = new List<string>(),
                    Errors = new List<string> { $"{failureMessage}: {response.ReasonPhrase}" },
                    ErrorType = ErrorType.InternalServerError
                };
            }
            catch (Exception ex)
            {
                return new PaginationResponseWrapper<List<string>>
                {
                    Data = new List<string>(),
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

    }

}
