using Application.DTOs;
using Application.DTOs.Responses;
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
                postResponseDTO.Media = post.MediaList.Select(media => new MediaDTO
                {
                    Url = media.MediaUrl,
                    Type = media.MediaType
                }).ToList();
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

            // Check if media is provided when HasMedia is true
            if (postInputDTO.HasMedia && (postInputDTO.Media == null || !postInputDTO.Media.Any()))
            {
                mediaUploadResponse.Success = false;
                mediaUploadResponse.Errors = new List<string> { "Media files are required when HasMedia is true" };
                return mediaUploadResponse;
            }

            // Check total media count (files + URLs)
            int totalMediaCount = (postInputDTO.Media?.Count() ?? 0) + (postInputDTO.MediaUrls?.Count ?? 0);
            if (totalMediaCount > 4)
            {
                mediaUploadResponse.Success = false;
                mediaUploadResponse.Errors = new List<string> { "Maximum of 4 media items allowed" };
                return mediaUploadResponse;
            }

            if (!postInputDTO.HasMedia)
            {
                mediaUploadResponse.Success = true;
                return mediaUploadResponse;
            }

            mediaUploadRequest.usageCategory = UsageCategory.Post;
            mediaUploadRequest.Files = postInputDTO.Media;
            mediaUploadRequest.MediaType = postInputDTO.MediaType;

            try
            {
                mediaUploadResponse = await _mediaServiceClient.UploadMediaAsync(mediaUploadRequest);
                return mediaUploadResponse;
            }
            catch (Exception ex)
            {
                mediaUploadResponse.Success = false;
                mediaUploadResponse.Errors = new List<string> { $"Failed to upload media: {ex.Message}" };
                return mediaUploadResponse;
            }
        }

        public async Task<ResponseWrapper<Post>> UpdatePostMedia(PostInputDTO postInputDto, Post postToUpdate)
        {
            var response = new ResponseWrapper<Post>();

            // Get existing and incoming media URLs
            var existingMediaUrls = postToUpdate.MediaList?.Select(m => m.MediaUrl).ToList() ?? new List<string>();
            var inputMediaUrls = postInputDto.MediaUrls ?? new List<string>();
            // URLs to be deleted are those that exist in the post but not in the input
            var urlsToBeDeleted = existingMediaUrls
                .Where(url => !inputMediaUrls.Contains(url))
                .ToList();
            // Determine various conditions
            bool hasNewMedia = postInputDto.Media != null && postInputDto.Media.Any();
            bool hasDeletedMedia = urlsToBeDeleted.Any();
            bool isMediaListEmpty = postToUpdate.MediaList == null || !postToUpdate.MediaList.Any();
            bool mediaTypeChanged = postToUpdate.MediaList != null && postToUpdate.MediaList.Any() &&
                postInputDto.MediaType != postToUpdate.MediaList.First().MediaType;

            // Case 1: No media in update (HasMedia is false)
            if (!postInputDto.HasMedia)
            {
                if (hasDeletedMedia)
                {
                    try
                    {
                        var deleteResult = await _mediaServiceClient.DeleteMediaAsync(urlsToBeDeleted);
                        if (!deleteResult)
                        {
                            response.Errors.Add("Failed to delete removed media.");
                            response.ErrorType = ErrorType.InternalServerError;
                            return response;
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Errors.Add($"Warning: Failed to delete media: {ex.Message}");
                        response.ErrorType = ErrorType.InternalServerError;
                    }

                    postToUpdate.MediaList.RemoveAll(m => urlsToBeDeleted.Contains(m.MediaUrl));
                }

                response.Data = postToUpdate;
                return response;
            }

            // Validate total media count
            int totalMediaCount = (postInputDto.Media?.Count() ?? 0) + inputMediaUrls.Count;
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
                    response.Errors.Add(uploadResult.Errors?.FirstOrDefault() ?? "Failed to upload the media.");
                    response.ErrorType = ErrorType.InternalServerError;
                    return response;
                }

                postToUpdate.MediaList = uploadResult.Urls
                    .Select(url => new Media
                    {
                        MediaType = postInputDto.MediaType,
                        MediaUrl = url
                    })
                    .ToList();
                response.Data = postToUpdate;
                return response;
            }

            // Case 3: Media type mismatch
            if (mediaTypeChanged)
            {
                response.Errors.Add("Invalid request. Cannot mix media types.");
                response.ErrorType = ErrorType.BadRequest;
                return response;
            }

            // Case 4: Handle simultaneous upload and delete
            if (hasNewMedia && hasDeletedMedia)
            {
                // First, upload new media
                var mediaUploadRequest = new MediaUploadRequest
                {
                    MediaType = postInputDto.MediaType,
                    Files = postInputDto.Media,
                    usageCategory = UsageCategory.Post
                };

                var uploadResult = await _mediaServiceClient.UploadMediaAsync(mediaUploadRequest);
                if (!uploadResult.Success)
                {
                    response.Errors.Add("Failed to upload new media.");
                    response.ErrorType = ErrorType.InternalServerError;
                    return response;
                }

                // Then, delete old media
                try
                {
                    var deleteResult = await _mediaServiceClient.DeleteMediaAsync(urlsToBeDeleted);
                    if (!deleteResult)
                    {
                        response.Errors.Add("Failed to delete old media.");
                        response.ErrorType = ErrorType.InternalServerError;
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    response.Errors.Add($"Warning: Failed to delete old media: {ex.Message}");
                    response.ErrorType = ErrorType.InternalServerError;
                }

                // Update the post's media list - keep only the URLs that are in the input
                postToUpdate.MediaList = postToUpdate.MediaList
                    .Where(m => inputMediaUrls.Contains(m.MediaUrl))
                    .ToList();

                // Add the newly uploaded media
                uploadResult.Urls.ForEach(url =>
                {
                    postToUpdate.MediaList.Add(new Media
                    {
                        MediaType = postInputDto.MediaType,
                        MediaUrl = url
                    });
                });

                response.Data = postToUpdate;
                return response;
            }

            // Case 5: Just adding new media to existing (no deletions)
            if (hasNewMedia)
            {
                var mediaUploadRequest = new MediaUploadRequest
                {
                    MediaType = postInputDto.MediaType,
                    Files = postInputDto.Media,
                    usageCategory = UsageCategory.Post
                };

                var uploadResult = await _mediaServiceClient.UploadMediaAsync(mediaUploadRequest);
                if (!uploadResult.Success)
                {
                    response.Errors.Add("Failed to upload new media.");
                    response.ErrorType = ErrorType.InternalServerError;
                    return response;
                }

                // Ensure MediaList is initialized
                if (postToUpdate.MediaList == null)
                    postToUpdate.MediaList = new List<Media>();

                // Keep only the URLs that are in the input
                postToUpdate.MediaList = postToUpdate.MediaList
                    .Where(m => inputMediaUrls.Contains(m.MediaUrl))
                    .ToList();

                // Add the newly uploaded media
                uploadResult.Urls.ForEach(url =>
                {
                    postToUpdate.MediaList.Add(new Media
                    {
                        MediaType = postInputDto.MediaType,
                        MediaUrl = url
                    });
                });
            }
            else if (hasDeletedMedia)
            {
                // Handle case where we're only deleting media
                try
                {
                    var deleteResult = await _mediaServiceClient.DeleteMediaAsync(urlsToBeDeleted);
                    if (!deleteResult)
                    {
                        response.Errors.Add("Failed to delete old media.");
                        response.ErrorType = ErrorType.InternalServerError;
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    response.Errors.Add($"Warning: Failed to delete old media: {ex.Message}");
                    response.ErrorType = ErrorType.InternalServerError;
                }

                // Keep only the URLs that are in the input
                postToUpdate.MediaList = postToUpdate.MediaList
                    .Where(m => inputMediaUrls.Contains(m.MediaUrl))
                    .ToList();
            }

            response.Data = postToUpdate;
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
                    Media = post.MediaList?.Select(m => new MediaDTO
                    {
                        Url = m.MediaUrl,
                        Type = m.MediaType
                    }).ToList() ?? new List<MediaDTO>(),
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
