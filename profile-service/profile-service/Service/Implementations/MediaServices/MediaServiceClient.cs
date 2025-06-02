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

            if (request?.File == null)
            {
                response.Errors = new List<string> { "File cannot be null." };
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }

            try
            {
                using var form = new MultipartFormDataContent();
                using var streamContent = new StreamContent(request.File.OpenReadStream());

                try
                {
                    streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
                        string.IsNullOrWhiteSpace(request.File.ContentType) ? "application/octet-stream" : request.File.ContentType);
                }
                catch (FormatException)
                {
                    response.Errors = new List<string> { "Invalid content type format." };
                    response.ErrorType = ErrorType.BadRequest;
                    return response;
                }

                // Add file content
                form.Add(streamContent, "Files", request.File.FileName);
                form.Add(new StringContent(request.MediaType.ToString()), "MediaType");
                form.Add(new StringContent(request.usageCategory.ToString()), "UsageCategory");

                try
                {
                    var httpResponse = await _http.PostAsync(_BASE_ROUTE, form, ct);
                    
                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        response.Errors = new List<string> { $"Media service returned error: {httpResponse.StatusCode} - {await httpResponse.Content.ReadAsStringAsync(ct)}" };
                        response.ErrorType = ErrorType.InternalServerError;
                        return response;
                    }

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
                    response.Errors = new List<string> { $"Failed to communicate with media service: {ex.Message}" };
                    response.ErrorType = ErrorType.InternalServerError;
                    return response;
                }
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

            if (newFile?.File == null)
            {
                response.Errors = new List<string> { "New file cannot be null." };
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }

            if (currentUrls == null)
            {
                response.Errors = new List<string> { "Current URLs cannot be null." };
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }

            try
            {
                using var form = new MultipartFormDataContent();
                using var streamContent = new StreamContent(newFile.File.OpenReadStream());

                try
                {
                    streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
                        string.IsNullOrWhiteSpace(newFile.File.ContentType) ? "application/octet-stream" : newFile.File.ContentType);
                }
                catch (FormatException)
                {
                    response.Errors = new List<string> { "Invalid content type format." };
                    response.ErrorType = ErrorType.BadRequest;
                    return response;
                }

                // Add file content
                form.Add(streamContent, "File", newFile.File.FileName);
                form.Add(new StringContent(newFile.MediaType.ToString()), "MediaType");

                // Add current URLs
                foreach (var url in currentUrls)
                {
                    form.Add(new StringContent(url), "MediaUrls");
                }

                try
                {
                    var httpResponse = await _http.PutAsync(_BASE_ROUTE, form, ct);
                    
                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        response.Errors = new List<string> { $"Media service returned error: {httpResponse.StatusCode} - {await httpResponse.Content.ReadAsStringAsync(ct)}" };
                        response.ErrorType = ErrorType.InternalServerError;
                        return response;
                    }

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
                    response.Errors = new List<string> { $"Failed to communicate with media service: {ex.Message}" };
                    response.ErrorType = ErrorType.InternalServerError;
                    return response;
                }
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

            if (urls == null || !urls.Any())
            {
                response.Errors = new List<string> { "URLs cannot be null or empty." };
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }

            try
            {
                var httpResponse = await _http.DeleteAsync($"{_BASE_ROUTE}?urls={string.Join(",", urls)}", ct);
                
                if (!httpResponse.IsSuccessStatusCode)
                {
                    response.Errors = new List<string> { $"Media service returned error: {httpResponse.StatusCode} - {await httpResponse.Content.ReadAsStringAsync(ct)}" };
                    response.ErrorType = ErrorType.InternalServerError;
                    return response;
                }

                var result = await httpResponse.Content.ReadFromJsonAsync<bool>(cancellationToken: ct);
                response.Data = result;
                response.Message = "Media deleted successfully";
                return response;
            }
            catch (HttpRequestException ex)
            {
                response.Errors = new List<string> { $"Failed to communicate with media service: {ex.Message}" };
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
