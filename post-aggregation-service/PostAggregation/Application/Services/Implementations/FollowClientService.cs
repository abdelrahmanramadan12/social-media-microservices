using Application.Services.Interfaces;
using Application.Configuration;

namespace Application.Services.Implementations
{
    using Application.DTOs;
    using Application.DTOs.Follow;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    public class FollowServiceClient : IFollowServiceClient
    {
        private readonly HttpClient _httpClient;

        public FollowServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }

        public async Task<ResponseWrapper<bool>> IsFollower(IsFollowerRequest request)
        {
            var result = new ResponseWrapper<bool>();

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"/api/follow/is-follower", request);

                if (!response.IsSuccessStatusCode)
                {
                    result.Errors = new List<string> { $"HTTP Error: {response.StatusCode}" };
                    return result;
                }

                var responseData = await response.Content.ReadFromJsonAsync<ResponseWrapper<bool>>();

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

        public async Task<ResponseWrapper<List<string>>> FilterFollowers(FilterFollowStatusRequest request)
        {
            var result = new ResponseWrapper<List<string>>();

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"/api/internal/follow/filter-followers", request);

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
                var response = await _httpClient.PostAsJsonAsync($"/api/follow/filter-following", request);

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
    }
}
