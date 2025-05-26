using Application.DTOs;
using Application.IServices;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MyCompany.MediaSdk;

internal sealed class MediaServiceClient : IMediaServiceClient
{
    private readonly HttpClient _http;

    public MediaServiceClient(HttpClient http)
    {
        this._http = http;
    }

    public async Task<MediaUploadResponse> UploadMediaAsync(MediaUploadRequest request, CancellationToken ct = default)
    {
        using var form = BuildMultipartContent(request);
        var response = await _http.PostAsync("media/upload", form, ct);
        return await ReadAs<MediaUploadResponse>(response, ct);
    }

    public async Task<MediaUploadResponse> EditMediaAsync(MediaUploadRequest mediaUploaRequest, IEnumerable<string> currentUrls, CancellationToken ct = default)
    {
        using var form = BuildMultipartContent(mediaUploaRequest);

        foreach (var url in currentUrls)
        {
            form.Add(new StringContent(url), "MediaUrls");
        }

        var response = await _http.PostAsync("media/edit", form, ct);
        return await ReadAs<MediaUploadResponse>(response, ct);
    }

    public async Task<bool> DeleteMediaAsync(IEnumerable<string> urls, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("media/delete", urls, ct);
        return await ReadAs<bool>(response, ct);
    }


    // Helper Methods
    private MultipartFormDataContent BuildMultipartContent(MediaUploadRequest req)
    {
        var form = new MultipartFormDataContent();

        // Add files
        foreach (var file in req.Files)
        {
            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
                string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType);

            // The controller’s action parameter is named "files"
            form.Add(streamContent, "files", file.FileName);
        }

        // Add media type
        form.Add(new StringContent(req.MediaType.ToString()), "mediaType");
        return form;
    }

    private static async Task<T> ReadAs<T>(HttpResponseMessage resp, CancellationToken ct)
    {
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<T>(cancellationToken: ct);

        return result ?? throw new InvalidOperationException(
            $"Response body for {typeof(T).Name} was empty or not valid JSON.");
    }
}
