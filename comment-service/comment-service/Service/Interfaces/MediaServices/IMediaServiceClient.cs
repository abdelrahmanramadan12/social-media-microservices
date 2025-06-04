using Service.DTOs.Requests;
using Service.DTOs.Responses;

namespace Service.Interfaces.MediaServices
{
    public interface IMediaServiceClient
    {
        /// <summary>
        /// Uploads a single media file to the media service.
        /// </summary>
        Task<MediaUploadResponseDto> UploadMediaAsync(
            MediaUploadRequestDto request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Edits existing media by uploading a new file and optionally deleting old ones.
        /// </summary>
        Task<MediaUploadResponseDto> EditMediaAsync(
            MediaUploadRequestDto newFile,
            IEnumerable<string> currentUrls,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes media files based on their URLs.
        /// </summary>
        Task<bool> DeleteMediaAsync(
            IEnumerable<string> urls,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Assigns media to a post input, typically used when creating a comment with media.
        /// </summary>
        Task<MediaUploadResponseDto> AssignMediaToPostInput(CreateCommentRequest commentInputDTO);
    }
}
