using Application.Configuration;
using Application.DTOs;
using Application.DTOs.Post;
using Application.Services.Interfaces;
using System.Net.Http.Json;

namespace Application.Services.Implementations
{
    public class PostServiceClient : IpostServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly PostServiceSettings _settings;

        public PostServiceClient(HttpClient httpClient, PostServiceSettings settings)
        {
            _httpClient = httpClient;
            _settings = settings;
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        }

        public async Task<ServiceResponse<PostResponseDTO>> GetPostByIdAsync(string postId)
        {
            var result = new ServiceResponse<PostResponseDTO>();

            try
            {
                var response = await _httpClient.GetAsync($"api/internal/posts/{postId}");

                if (!response.IsSuccessStatusCode)
                {
                    result.IsValid = false;
                    result.Errors = new List<string> { $"HTTP Error: {response.StatusCode}" };
                    return result;
                }

                var responseData = await response.Content.ReadFromJsonAsync<ApiResponse<PostResponseDTO>>();

                if (responseData?.Data != null)
                {
                    result.IsValid = true;
                    result.DataItem = responseData.Data;
                }
                else
                {
                    result.IsValid = false;
                    result.Errors = new List<string> { "Null response from post service." };
                }
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.Errors = new List<string> { ex.Message };
            }

            return result;
        }

        public async Task<ServiceResponse<PostResponseDTO>> GetProfilePostListAsync(string userId, string profileUserId, int pageSize, string nextCursor)
        {
            var result = new ServiceResponse<PostResponseDTO>();

            try
            {
                var request = new GetProfilePostListRequest
                {
                    UserId = userId,
                    NextCursor = nextCursor
                };

                var response = await _httpClient.PostAsJsonAsync($"api/internal/posts/user/{profileUserId}", request);

                if (!response.IsSuccessStatusCode)
                {
                    result.IsValid = false;
                    result.Errors = new List<string> { $"HTTP Error: {response.StatusCode}" };
                    return result;
                }

                var responseData = await response.Content.ReadFromJsonAsync<ApiResponse<List<PostResponseDTO>>>();

                if (responseData?.Data != null)
                {
                    result.IsValid = true;
                    result.DataList = responseData.Data;
                    result.NextCursor = responseData.Next;
                }
                else
                {
                    result.IsValid = false;
                    result.Errors = new List<string> { "Null response from post service." };
                }
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.Errors = new List<string> { ex.Message };
            }

            return result;
        }

        public async Task<ServiceResponse<PostResponseDTO>> GetPostListAsync(string userId, List<string> postIds)
        {
            var result = new ServiceResponse<PostResponseDTO>();

            try
            {
                var request = new GetPostListRequest
                {
                    UserId = userId,
                    PostIds = postIds
                };

                var response = await _httpClient.PostAsJsonAsync("api/internal/posts/list", request);

                if (!response.IsSuccessStatusCode)
                {
                    result.IsValid = false;
                    result.Errors = new List<string> { $"HTTP Error: {response.StatusCode}" };
                    return result;
                }

                var responseData = await response.Content.ReadFromJsonAsync<ApiResponse<List<PostResponseDTO>>>();

                if (responseData?.Data != null)
                {
                    result.IsValid = true;
                    result.DataList = responseData.Data;
                }
                else
                {
                    result.IsValid = false;
                    result.Errors = new List<string> { "Null response from post service." };
                }
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.Errors = new List<string> { ex.Message };
            }

            return result;
        }
    }

    internal class ApiResponse<T>
    {
        public T Data { get; set; }
        public string Next { get; set; }
    }
}
