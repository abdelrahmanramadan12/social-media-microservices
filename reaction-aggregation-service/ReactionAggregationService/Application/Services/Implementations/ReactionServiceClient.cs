using Application.Configurations;
using Application.DTOs;
using Application.DTOs.Reactions;
using Application.Services.Interfaces;
using System.Net.Http.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class ReactionServiceClient : IReactionServiceClient
    {
        private readonly HttpClient _httpClient;
        private const string BASE_ENDPOINT = "api/internal/reacts/post";
        private readonly ReactionServiceSettings _settings;

        public ReactionServiceClient(HttpClient httpClient, ReactionServiceSettings settings)
        {
            _httpClient = httpClient;
            _settings = settings;
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        }

        public async Task<PaginationResponseWrapper<List<string>>> GetReactsOfPostAsync(GetReactsOfPostRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.PostId))
            {
                return new PaginationResponseWrapper<List<string>>
                {
                    Errors = new List<string> { "Post ID is required." },
                    Message = "Invalid input.",
                    HasMore = false
                };
            }
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BASE_ENDPOINT}", request);
                return await HandleResponse<List<string>>(response, "Failed to retrieve post reactions.");
            }
            catch (Exception ex)
            {
                return new PaginationResponseWrapper<List<string>>
                {
                    Errors = new List<string> { ex.Message },
                    Message = "Unhandled exception occurred.",
                    HasMore = false
                };
            }
        }

        private async Task<PaginationResponseWrapper<T>> HandleResponse<T>(HttpResponseMessage response, string failureMessage)
        {
            var result = new PaginationResponseWrapper<T>();

            if (!response.IsSuccessStatusCode)
            {
                result.Errors = new List<string> { $"HTTP Error: {response.StatusCode}" };
                result.Message = failureMessage;
                return result;
            }

            try
            {
                var responseData = await response.Content.ReadFromJsonAsync<PaginationResponseWrapper<T>>();

                if (responseData != null)
                {
                    result.Data = responseData.Data;
                    result.Errors = responseData.Errors ?? new List<string>();
                    result.Message = responseData.Message ?? "Operation completed successfully.";
                    result.HasMore = responseData.HasMore;
                    result.Next = responseData.Next;
                }
                else
                {
                    result.Errors = new List<string> { "Null response from reaction service." };
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
