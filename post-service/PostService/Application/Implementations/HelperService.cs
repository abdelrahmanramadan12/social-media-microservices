using Application.DTOs;
using Application.IServices;
using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using Domain.ValueObjects;

namespace Application.Services
{
    public class HelperService : IHelperService
    {
        private readonly IMediaServiceClient _mediaServiceClient;
        public HelperService(IPostRepository postRepository, IValidationService validationService, IMediaServiceClient mediaServiceClient)
        {
            this._mediaServiceClient = mediaServiceClient;
        }

        // Helper Methods
        public MappingResult<PostResponseDTO> MapPostToPostResponseDto(Post post)
        {
            PostResponseDTO postResponseDTO = new PostResponseDTO();
            MappingResult<PostResponseDTO> mappingResult = new MappingResult<PostResponseDTO>();

            // Mapping Media
            if (post.MediaList != null && post.MediaList.Count() > 0)
            {
                postResponseDTO.MediaUrls = new List<string>();
                post.MediaList.ForEach(media =>
                {
                    postResponseDTO.MediaUrls.Add(media.MediaUrl);
                });
            }

            // Mapping Other Properties
            postResponseDTO.PostId = post.Id;
            postResponseDTO.AuthorId = post.AuthorId;
            postResponseDTO.PostContent = post.Content;
            postResponseDTO.NumberOfLikes = post.NumberOfLikes;
            postResponseDTO.NumberOfComments = post.NumberOfComments;
            postResponseDTO.Privacy = post.Privacy;
            postResponseDTO.CreatedAt = post.CreatedAt;
            postResponseDTO.IsEdited = !(post.UpdatedAt == null);

            mappingResult.Item = postResponseDTO;
            return mappingResult;
        }
        
        public async Task<MediaUploadResponse> AssignMediaToPostInput(PostInputDTO postInputDTO)
        {
            var mediaUploadResponse = new MediaUploadResponse();
            var mediaUploadRequest = new MediaUploadRequest();

            if (!postInputDTO.HasMedia || postInputDTO.Media == null || !postInputDTO.Media.Any())
            {
                mediaUploadResponse.Success = false;
                return mediaUploadResponse;
            }

            mediaUploadRequest.usageCategory = UsageCategory.Post;
            mediaUploadRequest.Files = postInputDTO.Media;

            switch (postInputDTO.MediaType)
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
                mediaUploadResponse = await _mediaServiceClient.UploadMediaAsync(mediaUploadRequest);
                return mediaUploadResponse;
            }
            catch
            {
                mediaUploadResponse.Success = false;
                return mediaUploadResponse;
            }
        }

        public async Task<ServiceResponse<Post>> UpdatePostMedia(PostInputDTO postInputDto, Post postToUpdate)
        {
            var response = new ServiceResponse<Post>();

            var existingMediaUrls = postToUpdate.MediaList?.Select(m => m.MediaUrl).ToList() ?? new List<string>();
            var inputMediaUrls = postInputDto.MediaUrls ?? new List<string>();
            var urlsToBeDeleted = existingMediaUrls
                .Where(url => !inputMediaUrls.Contains(url))
                .ToList();

            bool hasNewMedia = postInputDto.Media != null && postInputDto.Media.Any();
            bool hasDeletedMedia = urlsToBeDeleted.Any();
            bool isMediaListEmpty = postToUpdate.MediaList == null || !postToUpdate.MediaList.Any();
            bool mediaTypeChanged = postToUpdate.MediaList != null && postToUpdate.MediaList.Any() &&
                                     postInputDto.MediaType != postToUpdate.MediaList.First().MediaType;

            // Case 1: No new media submitted
            if (!postInputDto.HasMedia)
            {
                if (hasDeletedMedia)
                {
                    var deleteResult = await _mediaServiceClient.DeleteMediaAsync(urlsToBeDeleted);
                    if (!deleteResult)
                    {
                        response.Errors.Add("Failed to delete removed media.");
                        response.ErrorType = ErrorType.InternalServerError;
                        return response;
                    }

                    postToUpdate.MediaList.RemoveAll(m => urlsToBeDeleted.Contains(m.MediaUrl));
                }

                response.DataItem = postToUpdate;
                return response;
            }

            // Validate total media count
            int totalMediaCount = postInputDto.Media.Count() + inputMediaUrls.Count();
            if (totalMediaCount > 4)
            {
                response.Errors.Add("Invalid request. A maximum of 4 media items is allowed.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }

            // Case 2: New media for empty post
            if (isMediaListEmpty)
            {
                var uploadResult = await AssignMediaToPostInput(postInputDto);
                if (!uploadResult.Success)
                {
                    response.Errors.Add("Failed to upload the media.");
                    response.ErrorType = ErrorType.InternalServerError;
                    return response;
                }

                response.DataItem = postToUpdate;
                return response;
            }

            // Case 3: Media type mismatch
            if (mediaTypeChanged)
            {
                if (!inputMediaUrls.Any())
                {
                    var mediaUploadRequest = new MediaUploadRequest
                    {
                        MediaType = postInputDto.MediaType,
                        Files = postInputDto.Media
                    };

                    var uploadResult = await _mediaServiceClient.EditMediaAsync(mediaUploadRequest, existingMediaUrls);
                    if (!uploadResult.Success)
                    {
                        response.Errors.Add("Failed to update media with new type.");
                        response.ErrorType = ErrorType.InternalServerError;
                        return response;
                    }

                    // Replace all media
                    postToUpdate.MediaList.Clear();
                    uploadResult.Urls.ForEach(url =>
                    {
                        postToUpdate.MediaList.Add(new Media
                        {
                            MediaType = postInputDto.MediaType,
                            MediaUrl = url
                        });
                    });

                    response.DataItem = postToUpdate;
                    return response;
                }

                response.Errors.Add("Invalid request. Cannot mix media types.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }

            // Case 4: Matched type but some media deleted
            if (hasDeletedMedia)
            {
                var mediaUploadRequest = new MediaUploadRequest
                {
                    MediaType = postInputDto.MediaType,
                    Files = postInputDto.Media
                };

                var uploadResult = await _mediaServiceClient.EditMediaAsync(mediaUploadRequest, urlsToBeDeleted);
                if (!uploadResult.Success)
                {
                    response.Errors.Add("Failed to update media.");
                    response.ErrorType = ErrorType.InternalServerError;
                    return response;
                }

                // Remove old and add new
                postToUpdate.MediaList.RemoveAll(m => urlsToBeDeleted.Contains(m.MediaUrl));
                uploadResult.Urls.ForEach(url =>
                {
                    postToUpdate.MediaList.Add(new Media
                    {
                        MediaType = postInputDto.MediaType,
                        MediaUrl = url
                    });
                });

                response.DataItem = postToUpdate;
                return response;
            }

            // Case 5: Just adding new media to existing (no deletions)
            if (hasNewMedia)
            {
                var mediaUploadRequest = new MediaUploadRequest
                {
                    MediaType = postInputDto.MediaType,
                    Files = postInputDto.Media
                };

                var uploadResult = await _mediaServiceClient.UploadMediaAsync(mediaUploadRequest);
                if (!uploadResult.Success)
                {
                    response.Errors.Add("Failed to upload new media.");
                    response.ErrorType = ErrorType.InternalServerError;
                    return response;
                }

                uploadResult.Urls.ForEach(url =>
                {
                    postToUpdate.MediaList.Add(new Media
                    {
                        MediaType = postInputDto.MediaType,
                        MediaUrl = url
                    });
                });
            }

            response.DataItem = postToUpdate;
            return response;
        }

        public List<PostResponseDTO> AgregatePostResponseList(List<Post> posts)
        {
            List<PostResponseDTO> postResponseDTOs = new List<PostResponseDTO>();

            foreach (var post in posts)
            {
                var postResponse = new PostResponseDTO
                {
                    AuthorId = post.AuthorId,
                    PostId = post.Id,
                    PostContent = post.Content,
                    Privacy = post.Privacy,
                    MediaUrls = post.MediaList?.Select(m => m.MediaUrl).ToList() ?? new List<string>(),
                    CreatedAt = post.CreatedAt,
                    IsEdited = post.UpdatedAt.HasValue && post.UpdatedAt.Value > post.CreatedAt,
                    NumberOfLikes = post.NumberOfLikes,
                    NumberOfComments = post.NumberOfComments
                };

                postResponseDTOs.Add(postResponse);
            }

            return postResponseDTOs;
        }
    }
}
