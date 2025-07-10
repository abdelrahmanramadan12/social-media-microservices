using Application.DTOs;

namespace Application.IServices
{
    public interface IMediaServiceClient
    {
        Task<MediaUploadResponse> UploadMediaAsync(
            MediaUploadRequest request,
            CancellationToken cancellationToken = default);
    }
}
