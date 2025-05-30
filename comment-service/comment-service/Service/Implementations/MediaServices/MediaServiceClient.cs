using System.Net.Http.Headers;
using System.Net.Http.Json;
using Domain.DTOs;
using Domain.Enums;
using Service.Interfaces.MediaServices;

namespace Service.Implementations.MediaServices
{
    public class MediaServiceClient : IMediaServiceClient
    {
        private readonly HttpClient _http;
        private const string BASE_ROUTE = "api/internal/media";

        public MediaServiceClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<MediaUploadResponseDto> UploadMediaAsync(MediaUploadRequestDto request, CancellationToken ct = default)
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

            var response = await _http.PostAsync(BASE_ROUTE, form, ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<MediaUploadResponseDto>(cancellationToken: ct)
                ?? throw new InvalidOperationException("Failed to deserialize media upload response");
        }

        public async Task<MediaUploadResponseDto> EditMediaAsync(MediaUploadRequestDto mediaUploadRequest, IEnumerable<string> currentUrls, CancellationToken ct = default)
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

            // Add current URLs
            foreach (var url in currentUrls)
            {
                form.Add(new StringContent(url), "MediaUrls");
            }

            var response = await _http.PutAsync(BASE_ROUTE, form, ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<MediaUploadResponseDto>(cancellationToken: ct)
                ?? throw new InvalidOperationException("Failed to deserialize media edit response");
        }

        public async Task<bool> DeleteMediaAsync(IEnumerable<string> urls, CancellationToken ct = default)
        {
            var request = new DeleteMediaRequestDto { Urls = urls.ToList() };
            var response = await _http.DeleteAsync($"{BASE_ROUTE}?urls={string.Join(",", urls)}", ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<bool>(cancellationToken: ct);
        }


        /// helper functions
        public async Task<MediaUploadResponseDto> AssignMediaToPostInput(CreateCommentRequestDto commentInputDTO)
        {
            var mediaUploadResponse = new MediaUploadResponseDto();
            var mediaUploadRequest = new MediaUploadRequestDto();

            if (!commentInputDTO.HasMedia || commentInputDTO.Media == null || !commentInputDTO.Media.Any())
            {
                mediaUploadResponse.Success = false;
                return mediaUploadResponse;
            }

            mediaUploadRequest.usageCategory = UsageCategory.Post;
            mediaUploadRequest.Files = commentInputDTO.Media;

            switch (commentInputDTO.MediaType)
            {
                case MediaType.Video:
                    mediaUploadRequest.MediaType = MediaType.Video;
                    break;
                case MediaType.Image:
                    mediaUploadRequest.MediaType = MediaType.Image;
                    break;
                case MediaType.Audio:
                    mediaUploadRequest.MediaType = MediaType.Audio;
                    break;
                case MediaType.Document:
                    mediaUploadRequest.MediaType = MediaType.Document;
                    break;
                default:
                    break;
            }

            try
            {
                mediaUploadResponse = await UploadMediaAsync(mediaUploadRequest);
                return mediaUploadResponse;
            }
            catch
            {
                mediaUploadResponse.Success = false;
                return mediaUploadResponse;
            }
        }

    }
}
