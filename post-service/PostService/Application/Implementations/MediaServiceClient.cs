using Application.DTOs;
using Application.IServices;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Application.Services;

public class MediaServiceClient : IMediaServiceClient
{
    private readonly HttpClient _http;
    private const string BASE_ROUTE = "api/internal/media";

    public MediaServiceClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<MediaUploadResponse> UploadMediaAsync(MediaUploadRequest request, CancellationToken ct = default)
    {
        using var form = new MultipartFormDataContent();

        // Add files
        foreach (var file in request.Files)
        {
            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
                string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType);
            form.Add(streamContent, "Files", file.FileName);
        }

        // Add media type
        form.Add(new StringContent(request.MediaType.ToString()), "MediaType");

        // Add usage category
        form.Add(new StringContent(request.usageCategory.ToString()), "UsageCategory");

        var response = await _http.PostAsync(BASE_ROUTE, form, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<MediaUploadResponse>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Failed to deserialize media upload response");
    }

    public async Task<MediaUploadResponse> EditMediaAsync(MediaUploadRequest mediaUploadRequest, IEnumerable<string> currentUrls, CancellationToken ct = default)
    {
        using var form = new MultipartFormDataContent();

        // Add files
        foreach (var file in mediaUploadRequest.Files)
        {
            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
                string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType);
            form.Add(streamContent, "Files", file.FileName);
        }

        // Add media type
        form.Add(new StringContent(mediaUploadRequest.MediaType.ToString()), "MediaType");

        // Add usage category
        form.Add(new StringContent(mediaUploadRequest.usageCategory.ToString()), "UsageCategory");

        // Add current URLs
        foreach (var url in currentUrls)
        {
            form.Add(new StringContent(url), "MediaUrls");
        }

        var response = await _http.PutAsync(BASE_ROUTE, form, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<MediaUploadResponse>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Failed to deserialize media edit response");
    }

    public async Task<bool> DeleteMediaAsync(IEnumerable<string> urls, CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, BASE_ROUTE)
        {
            Content = JsonContent.Create(urls.ToList())
        };

        var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<bool>(cancellationToken: ct);
    }
}
