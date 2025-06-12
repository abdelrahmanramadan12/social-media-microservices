using Application.DTOs;
using Application.DTOs.Post;
using Application.Services.Interfaces;
using System.Net.Http.Json;

namespace Application.Services.Implementations
{
    public class PostServiceClient : IPostServiceClient
    {
        private readonly HttpClient _httpClient;
        private const string GET_POST_BY_ID_ENDPOINT = "api/internal/posts/{0}";
        private const string GET_PROFILE_POSTS_ENDPOINT = "api/internal/posts/user";
        private const string GET_POST_LIST_ENDPOINT = "api/internal/posts/list";

        public PostServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }

        public async Task<ResponseWrapper<PostResponseDTO>> GetPostByIdAsync(string postId)
        {
            try
            {
                var response = await _httpClient.GetAsync(string.Format(GET_POST_BY_ID_ENDPOINT, postId));
                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseWrapper<PostResponseDTO>
                    {
                        Errors = new List<string> { $"Failed to get post: {response.StatusCode}" },
                        ErrorType = ErrorType.InternalServerError
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ResponseWrapper<PostResponseDTO>>();
                return result ?? new ResponseWrapper<PostResponseDTO>
                {
                    Errors = new List<string> { "Empty response from post service" },
                    ErrorType = ErrorType.InternalServerError
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<PostResponseDTO>
                {
                    Errors = new List<string> { $"Error getting post: {ex.Message}" },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<PaginationResponseWrapper<List<PostResponseDTO>>> GetProfilePostListAsync(string userId, string profileUserId, string next)
        {
            try
            {
                var request = new GetProfilePostListRequest
                {
                    ProfileUserId = profileUserId,
                    UserId = userId,
                    Next = next
                };

                var response = await _httpClient.PostAsJsonAsync(GET_PROFILE_POSTS_ENDPOINT, request);

                if (!response.IsSuccessStatusCode)
                {
                    return new PaginationResponseWrapper<List<PostResponseDTO>>
                    {
                        Errors = new List<string> { $"Failed to get profile posts: {response.StatusCode}" },
                        ErrorType = ErrorType.InternalServerError
                    };
                }

                var x = await response.Content.ReadAsStringAsync();
                var result = await response.Content.ReadFromJsonAsync<PaginationResponseWrapper<List<PostResponseDTO>>>();
                return result ?? new PaginationResponseWrapper<List<PostResponseDTO>>
                {
                    Errors = new List<string> { "Empty response from post service" },
                    ErrorType = ErrorType.InternalServerError
                };
            }
            catch (Exception ex)
            {
                return new PaginationResponseWrapper<List<PostResponseDTO>>
                {
                    Errors = new List<string> { $"Error getting profile posts: {ex.Message}" },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<ResponseWrapper<List<PostResponseDTO>>> GetPostListAsync(string userId, List<string> postIds)
        {
            try
            {
                var request = new GetPostListRequest
                {
                    UserId = userId,
                    PostIds = postIds
                };

                var response = await _httpClient.PostAsJsonAsync(GET_POST_LIST_ENDPOINT, request);
                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseWrapper<List<PostResponseDTO>>
                    {
                        Errors = new List<string> { $"Failed to get posts: {response.StatusCode}" },
                        ErrorType = ErrorType.InternalServerError
                    };
                }
                var result = await response.Content.ReadFromJsonAsync<ResponseWrapper<List<PostResponseDTO>>>();
                return result ?? new ResponseWrapper<List<PostResponseDTO>>
                {
                    Errors = new List<string> { "Empty response from post service" },
                    ErrorType = ErrorType.InternalServerError
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<List<PostResponseDTO>>
                {
                    Errors = new List<string> { $"Error getting posts: {ex.Message}" },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }
    }
}
