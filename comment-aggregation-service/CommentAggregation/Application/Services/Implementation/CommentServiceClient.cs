using Application.DTOs.Application.DTOs;
using Application.DTOs.Comment;
using Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Application.Services.Implementation
{
    public class CommentServiceClient : ICommentServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private const string BASE_ENDPOINT = "/api/internal/comment/list";

        public CommentServiceClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["Clients:Comment"] ?? throw new ArgumentNullException("Clients:Comment");
        }

        public async Task<PaginationResponseWrapper<List<CommentResponse>>> GetPagedCommentList(GetPagedCommentRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_baseUrl.TrimEnd('/') + BASE_ENDPOINT, request);
                if (!response.IsSuccessStatusCode)
                {
                    return new PaginationResponseWrapper<List<CommentResponse>>
                    {
                        Errors = new List<string> { $"HTTP Error: {response.StatusCode}" },
                        Message = "Failed to retrieve comments."
                    };
                }
                var responseData = await response.Content.ReadFromJsonAsync<PaginationResponseWrapper<List<CommentResponse>>>();
                return responseData ?? new PaginationResponseWrapper<List<CommentResponse>>
                {
                    Errors = new List<string> { "Null response from comment service." },
                    Message = "Failed to retrieve comments."
                };
            }
            catch (Exception ex)
            {
                return new PaginationResponseWrapper<List<CommentResponse>>
                {
                    Errors = new List<string> { ex.Message },
                    Message = "Exception occurred while retrieving comments."
                };
            }
        }
    }
}
