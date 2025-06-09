using Application.Configuration;
using Application.DTOs;
using Application.DTOs.Reaction;
using Application.Services.Interfaces;
using System.Net.Http.Json;

namespace Application.Services.Implementations
{
    public class ReactionServiceClient : IReactionServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ReactionServiceSettings _settings;

        private const string FILTER_POSTS_ENDPOINT = "/api/internal/reacts/post/filter";
        private const string GET_POSTS_BY_USER_ENDPOINT = "/api/internal/reacts/post/user";
        private const string GET_REACTS_OF_POST_ENDPOINT = "/api/internal/reacts/post";
        private const string IS_POST_LIKED_BY_USER_ENDPOINT = "/api/internal/reacts/post/is-liked";

        public ReactionServiceClient(HttpClient httpClient, ReactionServiceSettings settings)
        {
            _httpClient = httpClient;
            _settings = settings;
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        }

        public async Task<ResponseWrapper<FilteredPostsReactedByUserResponse>> FilterPostsReactedByUserAsync(FilterPostsReactedByUserRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(FILTER_POSTS_ENDPOINT, request);
                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseWrapper<FilteredPostsReactedByUserResponse>
                    {
                        Errors = new List<string> { $"Failed to filter posts: {response.StatusCode}" },
                        ErrorType = ErrorType.InternalServerError
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ResponseWrapper<FilteredPostsReactedByUserResponse>>();
                return result ?? new ResponseWrapper<FilteredPostsReactedByUserResponse>
                {
                    Errors = new List<string> { "Empty response from reaction service" },
                    ErrorType = ErrorType.InternalServerError
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<FilteredPostsReactedByUserResponse>
                {
                    Errors = new List<string> { $"Error filtering posts: {ex.Message}" },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<ResponseWrapper<GetPostsReactedByUserResponse>> GetPostsReactedByUserAsync(GetPostsReactedByUserRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(GET_POSTS_BY_USER_ENDPOINT, request);
                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseWrapper<GetPostsReactedByUserResponse>
                    {
                        Errors = new List<string> { $"Failed to get reacted posts: {response.StatusCode}" },
                        ErrorType = ErrorType.InternalServerError
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ResponseWrapper<GetPostsReactedByUserResponse>>();
                return result ?? new ResponseWrapper<GetPostsReactedByUserResponse>
                {
                    Errors = new List<string> { "Empty response from reaction service" },
                    ErrorType = ErrorType.InternalServerError
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<GetPostsReactedByUserResponse>
                {
                    Errors = new List<string> { $"Error getting reacted posts: {ex.Message}" },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<ResponseWrapper<GetReactsOfPostResponse>> GetReactsOfPostAsync(GetReactsOfPostRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(GET_REACTS_OF_POST_ENDPOINT, request);
                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseWrapper<GetReactsOfPostResponse>
                    {
                        Errors = new List<string> { $"Failed to get post reactions: {response.StatusCode}" },
                        ErrorType = ErrorType.InternalServerError
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ResponseWrapper<GetReactsOfPostResponse>>();
                return result ?? new ResponseWrapper<GetReactsOfPostResponse>
                {
                    Errors = new List<string> { "Empty response from reaction service" },
                    ErrorType = ErrorType.InternalServerError
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<GetReactsOfPostResponse>
                {
                    Errors = new List<string> { $"Error getting post reactions: {ex.Message}" },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }
        
        public async Task<ResponseWrapper<bool>> IsPostLikedByUser(IsPostLikedByUserRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(IS_POST_LIKED_BY_USER_ENDPOINT, request);
                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseWrapper<bool>
                    {
                        Errors = new List<string> { $"Failed to check if post is liked: {response.StatusCode}" },
                        ErrorType = ErrorType.InternalServerError
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ResponseWrapper<bool>>();
                return result ?? new ResponseWrapper<bool>
                {
                    Errors = new List<string> { "Empty response from reaction service" },
                    ErrorType = ErrorType.InternalServerError
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<bool>
                {
                    Errors = new List<string> { $"Error checking if post is liked: {ex.Message}" },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }
    }
}

