using Application.Services.Interfaces;

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

        public async Task<ServiceResponseDTO<bool>> IsFollower(IsFollowerRequest request)
        {
            var result = new ServiceResponseDTO<bool>();

            try
            {
                // Change this URL to the actual one you're calling
                var response = await _httpClient.PostAsJsonAsync("https://sm-follow-service.com/api/follow/is-follower", request);

                if (!response.IsSuccessStatusCode)
                {
                    result.Success = false;
                    result.Errors = new List<string> { $"HTTP Error: {response.StatusCode}" };
                    return result;
                }

                var responseData = await response.Content.ReadFromJsonAsync<IsFollwerResponse>();

                if (responseData is not null)
                {
                    result.Success = responseData.success;
                    result.Item = responseData.IsFollwer;
                    result.Errors = responseData.Errors ?? new List<string>();
                }
                else
                {
                    result.Success = false;
                    result.Errors = new List<string> { "Null response from follow service." };
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
