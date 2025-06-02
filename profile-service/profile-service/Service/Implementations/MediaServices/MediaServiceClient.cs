using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Service.DTOs;
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

        public async Task<MediaUploadResponseDto> UploadMediaAsync(MediaUploadRequestDto request, CancellationToken ct = default)
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

            var response = await _http.PostAsync(_BASE_ROUTE, form, ct);
            response.EnsureSuccessStatusCode();

            var res = await response.Content.ReadFromJsonAsync<MediaUploadResponseDto>(cancellationToken: ct);

            return res ?? throw new InvalidOperationException("Failed to deserialize media upload response");
        }



        public async Task<MediaUploadResponseDto> EditMediaAsync(MediaUploadRequestDto newFile, IEnumerable<string> currentUrls, CancellationToken ct = default)
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

            var response = await _http.PutAsync(_BASE_ROUTE, form, ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<MediaUploadResponseDto>(cancellationToken: ct)
                   ?? throw new InvalidOperationException("Failed to deserialize media edit response");
        }

        public async Task<bool> DeleteMediaAsync(IEnumerable<string> urls, CancellationToken ct = default)
        {
            var response = await _http.DeleteAsync($"{_BASE_ROUTE}?urls={string.Join(",", urls)}", ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<bool>(cancellationToken: ct);
        }
    }
}
