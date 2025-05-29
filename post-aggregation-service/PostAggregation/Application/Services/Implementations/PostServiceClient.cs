using Application.DTOs.Post;
using Application.Services.Interfaces;
using System.Net.Http.Json;

public class PostServiceClient : IpostServiceClient
{
    private readonly HttpClient _httpClient;

    public PostServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ProfilePostsResponse> GetProfilePosts(ProfilePostsRequest request)
    {
        var result = new ProfilePostsResponse
        {
            Posts = new List<Post>(),
            Errors = new List<string>()
        };

        try
        {
            // Replace with your actual endpoint
            var response = await _httpClient.PostAsJsonAsync("https://sm-posts-service.com/api/posts/profile", request);

            if (!response.IsSuccessStatusCode)
            {
                result.Success = false;
                result.Errors.Add($"HTTP Error: {response.StatusCode}");
                return result;
            }

            var responseData = await response.Content.ReadFromJsonAsync<ProfilePostsResponse>();

            if (responseData is not null)
            {
                return responseData;
            }

            result.Success = false;
            result.Errors.Add("Null response from post service.");
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add(ex.Message);
        }

        return result;
    }
}
