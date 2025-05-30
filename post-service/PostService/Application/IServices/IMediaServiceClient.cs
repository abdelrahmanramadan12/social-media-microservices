using Application.DTOs;

namespace Application.IServices
{
    public interface IMediaServiceClient
    {
        /// <summary>
        /// Uploads media files to the media service.
        /// </summary>
        Task<MediaUploadResponse> UploadMediaAsync(
            MediaUploadRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Edits existing media by uploading new files and optionally deleting old ones.
        /// </summary>
        Task<MediaUploadResponse> EditMediaAsync(
            MediaUploadRequest newFiles,
            IEnumerable<string> currentUrls,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes media files based on their URLs.
        /// </summary>
        Task<bool> DeleteMediaAsync(
            IEnumerable<string> urls,
            CancellationToken cancellationToken = default);
    }
}
