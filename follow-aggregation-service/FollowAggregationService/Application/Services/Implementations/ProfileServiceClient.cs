using Application.DTOs;
using Application.DTOs.Profile;
using Application.Services.Interfaces;
using System.Net.Http.Json;

namespace Application.Services.Implementations
{
    public class ProfileServiceClient : IProfileServiceClient
    {
        private readonly HttpClient _httpClient;
        private const string BASE_ENDPOINT = "api/internal/profile";

        public ProfileServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseWrapper<SimpleUserProfile>> GetByUserIdMinAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new ResponseWrapper<SimpleUserProfile>
                {
                    Errors = new List<string> { "User ID is required." },
                    ErrorType = ErrorType.BadRequest,
                    Message = "Invalid input. user ID is missing"
                };
            }

            try
            {
                var response = await _httpClient.GetAsync($"{BASE_ENDPOINT}/min/id/{userId}");
                return await HandleResponse<SimpleUserProfile>(response, "Failed to retrieve user profile.");
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<SimpleUserProfile>
                {
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.BadRequest,
                    Message = "Unhandled exception occurred."
                };
            }
        }

        public async Task<ResponseWrapper<List<SimpleUserProfile>>> GetUsersByIdsAsync(GetUsersProfileByIdsRequest request)
        {
            if (request == null || request.UserIds == null || request.UserIds.Count == 0)
            {
                return new ResponseWrapper<List<SimpleUserProfile>>
                {
                    Errors = new List<string> { "User IDs are required." },
                    Message = "Invalid input."
                };
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BASE_ENDPOINT}/list", request);
                return await HandleResponse<List<SimpleUserProfile>>(response, "Failed to retrieve user profiles.");
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<List<SimpleUserProfile>>
                {
                    Errors = new List<string> { ex.Message },
                    Message = "Unhandled exception occurred."
                };
            }
        }

        // Shared response handler
        private async Task<ResponseWrapper<T>> HandleResponse<T>(HttpResponseMessage response, string failureMessage)
        {
            var result = new ResponseWrapper<T>();

            if (!response.IsSuccessStatusCode)
            {
                result.Errors = new List<string> { $"HTTP Error: {response.StatusCode}" };
                result.Message = failureMessage;
                return result;
            }

            try
            {
                var responseData = await response.Content.ReadFromJsonAsync<ResponseWrapper<T>>();

                if (responseData != null)
                {
                    result.Data = responseData.Data;
                    result.Errors = responseData.Errors ?? new List<string>();
                    result.Message = responseData.Message ?? "Operation completed successfully.";
                }
                else
                {
                    result.Errors = new List<string> { "Null response from profile service." };
                    result.Message = failureMessage;
                }
            }
            catch (Exception ex)
            {
                result.Errors = new List<string> { $"Deserialization error: {ex.Message}" };
                result.Message = failureMessage;
            }

            return result;
        }
    }
}
