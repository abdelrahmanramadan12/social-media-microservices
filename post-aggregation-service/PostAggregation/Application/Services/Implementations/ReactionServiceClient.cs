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

        public ReactionServiceClient(HttpClient httpClient, ReactionServiceSettings settings)
        {
            _httpClient = httpClient;
            _settings = settings;
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        }

        private async Task<ResponseWrapper<T>> SendRequestAsync<T>(string endpoint, object request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, request);

                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseWrapper<T>
                    {
                        Errors = new List<string>
                        {
                            $"Reaction service returned status code {(int)response.StatusCode} - {response.ReasonPhrase}"
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

        public async Task<ResponseWrapper<FilteredPostsReactedByUserResponse>> FilterPostsReactedByUserAsync(FilterPostsReactedByUserRequest request)
        {
            return await SendRequestAsync<FilteredPostsReactedByUserResponse>(FILTER_POSTS_ENDPOINT, request);
        }

        public async Task<ResponseWrapper<GetPostsReactedByUserResponse>> GetPostsReactedByUserAsync(GetPostsReactedByUserRequest request)
        {
            return await SendRequestAsync<GetPostsReactedByUserResponse>(GET_POSTS_BY_USER_ENDPOINT, request);
        }

        public async Task<ResponseWrapper<GetReactsOfPostResponse>> GetReactsOfPostAsync(GetReactsOfPostRequest request)
        {
            return await SendRequestAsync<GetReactsOfPostResponse>(GET_REACTS_OF_POST_ENDPOINT, request);
        }
    }
}

