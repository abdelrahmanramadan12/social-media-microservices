using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Service.DTOs;
using Service.DTOs.Requests;
using Service.DTOs.Responses;
using Service.Interfaces.MediaServices;

namespace Service.Implementations.MediaServices
{
    public class MediaServiceClient : IMediaServiceClient
    {
        private readonly HttpClient _http;
        private const string _BASE_ROUTE = "/api/internal/media";

        public MediaServiceClient(HttpClient http, IConfiguration configuration)
        {
            _http = http;
        }

        public async Task<ResponseWrapper<MediaUploadResponseDto>> UploadMediaAsync(MediaUploadRequestDto request, CancellationToken ct = default)
        {
            var response = new ResponseWrapper<MediaUploadResponseDto>();

            try
            {
                using var form = new MultipartFormDataContent();

                // Important: Use "Files" to match ReceivedMediaDto
                var streamContent = new StreamContent(request.File.OpenReadStream());
                streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
                    string.IsNullOrWhiteSpace(request.File.ContentType) ? "application/octet-stream" : request.File.ContentType);
                form.Add(streamContent, "Files", request.File.FileName);

                // Add MediaType and UsageCategory
                form.Add(new StringContent(request.MediaType.ToString()), "MediaType");
                form.Add(new StringContent(request.usageCategory.ToString()), "UsageCategory");

                var httpResponse = await _http.PostAsync(_BASE_ROUTE, form, ct);
                httpResponse.EnsureSuccessStatusCode();

                var result = await httpResponse.Content.ReadFromJsonAsync<MediaUploadResponseDto>(cancellationToken: ct);
                if (result == null)
                {
                    response.Errors = new List<string> { "Failed to deserialize media upload response" };
                    response.ErrorType = ErrorType.InternalServerError;
                    return response;
                }

                response.Data = result;
                response.Message = "Media uploaded successfully";
                return response;
            }
            catch (HttpRequestException ex)
            {
                response.Errors = new List<string> { $"Failed to upload media: {ex.Message}" };
                response.ErrorType = ErrorType.InternalServerError;
                return response;
            }
            catch (Exception ex)
            {
                response.Errors = new List<string> { $"An unexpected error occurred while uploading media: {ex.Message}" };
                response.ErrorType = ErrorType.InternalServerError;
                return response;
            }
        }

        public async Task<ResponseWrapper<MediaUploadResponseDto>> EditMediaAsync(MediaUploadRequestDto newFile, IEnumerable<string> currentUrls, CancellationToken ct = default)
        {
            var response = new ResponseWrapper<MediaUploadResponseDto>();

            try
            {
                using var form = new MultipartFormDataContent();

                // Add single file
                var streamContent = new StreamContent(newFile.File.OpenReadStream());
                streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
                    string.IsNullOrWhiteSpace(newFile.File.ContentType) ? "application/octet-stream" : newFile.File.ContentType);
                form.Add(streamContent, "File", newFile.File.FileName);

                // Add media type
                form.Add(new StringContent(newFile.MediaType.ToString()), "MediaType");

                // Add current URLs
                foreach (var url in currentUrls)
                {
                    form.Add(new StringContent(url), "MediaUrls");
                }

                var httpResponse = await _http.PutAsync(_BASE_ROUTE, form, ct);
                httpResponse.EnsureSuccessStatusCode();

                var result = await httpResponse.Content.ReadFromJsonAsync<MediaUploadResponseDto>(cancellationToken: ct);
                if (result == null)
                {
                    response.Errors = new List<string> { "Failed to deserialize media edit response" };
                    response.ErrorType = ErrorType.InternalServerError;
                    return response;
                }

                response.Data = result;
                response.Message = "Media updated successfully";
                return response;
            }
            catch (HttpRequestException ex)
            {
                response.Errors = new List<string> { $"Failed to edit media: {ex.Message}" };
                response.ErrorType = ErrorType.InternalServerError;
                return response;
            }
            catch (Exception ex)
            {
                response.Errors = new List<string> { $"An unexpected error occurred while editing media: {ex.Message}" };
                response.ErrorType = ErrorType.InternalServerError;
                return response;
            }
        }

        public async Task<ResponseWrapper<bool>> DeleteMediaAsync(IEnumerable<string> urls, CancellationToken ct = default)
        {
            var response = new ResponseWrapper<bool>();

            try
            {
                var httpResponse = await _http.DeleteAsync($"{_BASE_ROUTE}?urls={string.Join(",", urls)}", ct);
                httpResponse.EnsureSuccessStatusCode();

                var result = await httpResponse.Content.ReadFromJsonAsync<bool>(cancellationToken: ct);
                response.Data = result;
                response.Message = "Media deleted successfully";
                return response;
            }
            catch (HttpRequestException ex)
            {
                response.Errors = new List<string> { $"Failed to delete media: {ex.Message}" };
                response.ErrorType = ErrorType.InternalServerError;
                return response;
            }
            catch (Exception ex)
            {
                response.Errors = new List<string> { $"An unexpected error occurred while deleting media: {ex.Message}" };
                response.ErrorType = ErrorType.InternalServerError;
                return response;
            }
        }
    }
}
