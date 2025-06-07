using Application.Configuration;
using Application.DTOs;
using Application.DTOs.Post;
using Application.Services.Interfaces;
using System.Net.Http.Json;

namespace Application.Services.Implementations
{
    public class PostServiceClient : IPostServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly PostServiceSettings _settings;

        private const string GET_POST_BY_ID_ENDPOINT = "api/internal/posts/{0}";
        private const string GET_PROFILE_POSTS_ENDPOINT = "api/internal/posts/user/{0}";
        private const string GET_POST_LIST_ENDPOINT = "api/internal/posts/list";

        public PostServiceClient(HttpClient httpClient, PostServiceSettings settings)
        {
            _httpClient = httpClient;
            _settings = settings;
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        }

        private async Task<ResponseWrapper<T>> SendRequestAsync<T>(string endpoint, object request = null)
        {
            try
            {
                HttpResponseMessage response;
                if (request != null)
                {
                    response = await _httpClient.PostAsJsonAsync(endpoint, request);
                }
                else
                {
                    response = await _httpClient.GetAsync(endpoint);
                }

                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseWrapper<T>
                    {
                        Errors = new List<string>
                        {
                            $"Post service returned status code {(int)response.StatusCode} - {response.ReasonPhrase}"
                        }
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ResponseWrapper<T>>();
                return result ?? new ResponseWrapper<T>
                {
                    Errors = new List<string> { "Response was null" }
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<T>
                {
                    Errors = new List<string> { $"Unhandled exception: {ex.Message}" }
                };
            }
        }

        public async Task<ResponseWrapper<PostResponseDTO>> GetPostByIdAsync(string postId)
        {
            return await SendRequestAsync<PostResponseDTO>(string.Format(GET_POST_BY_ID_ENDPOINT, postId));
        }

        public async Task<ResponseWrapper<List<PostResponseDTO>>> GetProfilePostListAsync(string userId, string profileUserId, int pageSize, string nextCursor)
        {
            var request = new GetProfilePostListRequest
            {
                UserId = userId,
                Next = nextCursor
            };
            return await SendRequestAsync<List<PostResponseDTO>>(string.Format(GET_PROFILE_POSTS_ENDPOINT, profileUserId), request);
        }

        public async Task<ResponseWrapper<List<PostResponseDTO>>> GetPostListAsync(string userId, List<string> postIds)
        {
            var request = new GetPostListRequest
            {
                UserId = userId,
                PostIds = postIds
            };
            return await SendRequestAsync<List<PostResponseDTO>>(GET_POST_LIST_ENDPOINT, request);
        }
    }
}
