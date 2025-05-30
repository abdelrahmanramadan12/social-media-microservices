using Application.DTOs.Reaction;
using System.Net.Http.Json;

namespace Application.Services.Implementations
{
    public class ReactionServiceClient
    {
        private readonly HttpClient _httpClient;

        public ReactionServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<ReactedPostListResponse> GetReactedPostsAsync(FilteredReactedPostListRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/reactions/posts-reacted-by-user", request);

                if (!response.IsSuccessStatusCode)
                {
                    return new ReactedPostListResponse
                    {
                        Success = false,
                        Errors = new List<string>
                        {
                            $"Reaction service returned status code {(int)response.StatusCode} - {response.ReasonPhrase}"
                        }
                    };
                }

                var result = await response.Content.ReadFromJsonAsync<ReactedPostListResponse>();

                if (result == null)
                {
                    return new ReactedPostListResponse
                    {
                        Success = false,
                        Errors = new List<string>
                        {
                            "Reaction service response was null or not in expected format."
                        }
                    };
                }

                if (result.ReactedPosts == null)
                {
                    result.ReactedPosts = new List<string>();
                    result.Errors.Add("ReactedPosts was null. Initialized to empty list.");
                }

                return result;
            }
            catch (Exception ex)
            {
                return new ReactedPostListResponse
                {
                    Success = false,
                    Errors = new List<string>
                    {
                        $"Unhandled exception: {ex.Message}"
                    }
                };
            }
        }
    }
}
