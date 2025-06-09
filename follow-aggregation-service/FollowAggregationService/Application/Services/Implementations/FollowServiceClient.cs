using Application.Configurations;
using Application.DTOs;
using Application.DTOs.Follow;
using Application.Services.Interfaces;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

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

        public async Task<ResponseWrapper<List<string>>> FilterFollowers(FilterFollowStatusRequest request)
        {
            var result = new ResponseWrapper<List<string>>();

            try
            {
                var response = await _httpClient.PostAsync(
                    $"{_settings.BaseUrl}/api/follow/filter-followers",
                    new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
                );

                if (!response.IsSuccessStatusCode)
                {
                    result.Errors = new List<string> { $"HTTP Error: {response.StatusCode}" };
                    return result;
                }

                var responseData = await response.Content.ReadFromJsonAsync<ResponseWrapper<List<string>>>();

                if (responseData is not null)
                {
                    result.Data = responseData.Data;
                    result.Errors = responseData.Errors ?? new List<string>();
                }
                else
                {
                    result.Errors = new List<string> { "Null response from follow service." };
                }
            }
            catch (Exception ex)
            {
                result.Errors = new List<string> { ex.Message };
            }

            return result;
        }

        public async Task<ResponseWrapper<List<string>>> FilterFollowing(FilterFollowStatusRequest request)
        {
            var result = new ResponseWrapper<List<string>>();

            try
            {
                var response = await _httpClient.PostAsync(
                    $"{_settings.BaseUrl}/api/follow/filter-following",
                    new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
                );

                if (!response.IsSuccessStatusCode)
                {
                    result.Errors = new List<string> { $"HTTP Error: {response.StatusCode}" };
                    return result;
                }

                var responseData = await response.Content.ReadFromJsonAsync<ResponseWrapper<List<string>>>();

                if (responseData is not null)
                {
                    result.Data = responseData.Data;
                    result.Errors = responseData.Errors ?? new List<string>();
                }
                else
                {
                    result.Errors = new List<string> { "Null response from follow service." };
                }
            }
            catch (Exception ex)
            {
                result.Errors = new List<string> { ex.Message };
            }

            return result;
        }

        public Task<PaginationResponseWrapper<List<string>>> GetFollowers(ListFollowPageRequest request)
        {
            return PostPaginationRequestAsync("/api/internal/follow/list-followers-page", request, "Failed to fetch followers");
        }

        public Task<PaginationResponseWrapper<List<string>>> GetFollowing(ListFollowPageRequest request)
        {
            return PostPaginationRequestAsync("/api/internal/follow/list-following-page", request, "Failed to fetch following");
        }

        private async Task<PaginationResponseWrapper<List<string>>> PostPaginationRequestAsync(string endpoint, ListFollowPageRequest request, string failureMessage)
        {
            try
            {
                var url = $"{_settings.BaseUrl}{endpoint}";
                var requestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, requestContent);

                var json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<PaginationResponseWrapper<List<string>>>(json);
                    if (result != null)
                        return result;
                    else
                        return new PaginationResponseWrapper<List<string>>
                        {
                            Data = new List<string>(),
                            Errors = new List<string> { "Failed to parse data from the follow service." },
                            ErrorType = ErrorType.InternalServerError
                        };
                }
                else
                {
                    // Try to extract error details from the response
                    try
                    {
                        var result = JsonSerializer.Deserialize<PaginationResponseWrapper<List<string>>>(json);
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
                            Errors = new List<string> { $"{failureMessage}: {response.ReasonPhrase}" },
                            ErrorType = ErrorType.InternalServerError
                        };
                    }
                }
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
