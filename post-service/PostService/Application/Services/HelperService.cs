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
        private readonly IReactionServiceClient _reactionServiceClient;
        private readonly IProfileServiceClient _profileServiceClient;
        public HelperService(IPostRepository postRepository, IValidationService validationService, IMediaServiceClient mediaServiceClient, IReactionServiceClient reactionServiceClient, IProfileServiceClient profileServiceClient)
        {
            this._mediaServiceClient = mediaServiceClient;
            this._reactionServiceClient = reactionServiceClient;
            this._profileServiceClient = profileServiceClient;
        }


        // Helper Methods
        public async Task<MappingResult<PostResponseDTO>> MapPostToPostResponseDto(Post post, string userId, bool checkIsLiked, bool assignProfile)
        {
            PostResponseDTO postResponseDTO = new PostResponseDTO();
            MappingResult<PostResponseDTO> mappingResult = new MappingResult<PostResponseDTO>();


            // IsLiked ??
            if (!checkIsLiked)
                postResponseDTO.IsLiked = false;
            else
            {
                postResponseDTO.IsLiked = true; // TODO: Replce with actual call to the service

            }

            // Assign Profile ??
            if (!assignProfile)
                postResponseDTO.PostAuthorProfile = null; // TODO: Add a try and catch here ... If there are smoe errors specify thier type
            else
            {
                postResponseDTO.PostAuthorProfile = new PostAuthorProfile()
                {
                    UserId = postResponseDTO.AuthorId,
                    ProfilePictureUrl = "",
                    DisplayName = "Test Test",
                    Username = "test.test"
                };
            }

            // Mapping Media
            if (post.MediaList.Count() > 0)
            {
                post.MediaList.ForEach(media =>
                {
                    postResponseDTO.MediaUrls.Add(media.MediaUrl);
                });
            }

            // Mapping Other Properties
            postResponseDTO.PostId = post.Id;
            postResponseDTO.AuthorId = post.AuthorId;
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

            if ((!postInputDTO.HasMedia) || (postInputDTO.Media.Count() <= 0))
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
        public async Task<ValidationResult> CheckPostAccess(string userId, Post post)
        {
            // LOGIC FLOW
            // Public(OK) OnlyMe(NO) 
            // Private(??) -> UserId is empty (NO)
            //             -> UserId Avilable (Follower ??) (OK) 
            //                                (Not Follower ??) (OK)  


            var result = new ValidationResult();
            if (post.Privacy == Privacy.Public)
                return result;

            if (post.Privacy == Privacy.OnlyMe)
            {
                result.Errors.Add("Invalid Operation! The post you are trying to access has been made private");
            }

            if (string.IsNullOrEmpty(userId))
            {
                result.Errors.Add("Invalid Operation! You don't have a permession fot the post you are trying to access");
                result.ErrorType = ErrorType.UnAuthorized;
            }

            try
            {
                bool IsFollower = await Task.FromResult(true); // TODO: Implement the follow service client and add the logic here
                return result;

            }
            catch
            {
                result.Errors.Add("Something wen't wrong while checking your perrmession. try again later!");
                result.ErrorType = ErrorType.InternalServerError;
                return result;
            }
        }
        public async Task<ServiceResponse<Post>> UpdatePostMedia(PostInputDTO postInputDto, Post postToUpdate)
        {
            var response = new ServiceResponse<Post>();

            var existingMediaUrls = postToUpdate.MediaList.Select(m => m.MediaUrl).ToList();
            var inputMediaUrls = postInputDto.MediaUrls ?? new List<string>();
            var urlsToBeDeleted = existingMediaUrls
                .Where(url => !inputMediaUrls.Contains(url))
                .ToList();

            bool hasNewMedia = postInputDto.Media != null && postInputDto.Media.Any();
            bool hasDeletedMedia = urlsToBeDeleted.Any();
            bool isMediaListEmpty = !postToUpdate.MediaList.Any();
            bool mediaTypeChanged = postToUpdate.MediaList.Any() &&
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
        public async Task<ReactedPostListResponse> GetReactedPostList(List<string> userPostIds)
        {
            var request = new ReactedPostListRequest()
            {
                PostIds = userPostIds
            };

            var response = await _reactionServiceClient.GetReactedPostsAsync(request);
            return response;
        }
        public List<PostResponseDTO> AgregatePostResponseList(List<Post> posts, List<PostAuthorProfile> profiles, List<string> likedPosts)
        {
            List<PostResponseDTO> postResponseDTOs = new List<PostResponseDTO>();

            foreach (var post in posts)
            {
                var profile = profiles.FirstOrDefault(p => p.UserId == post.AuthorId);

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
                    NumberOfComments = post.NumberOfComments,
                    IsLiked = likedPosts.Contains(post.Id),
                    PostAuthorProfile = profile
                };

                postResponseDTOs.Add(postResponse);
            }

            return postResponseDTOs;
        }
        public async Task<ReactedPostListResponse> GetReactedPostList(List<string> userPosts, string userId)
        {
            var request = new ReactedPostListRequest
            {
                PostIds = userPosts,
                UserId = userId
            };
            var response = await _reactionServiceClient.GetReactedPostsAsync(request);
            return response;
        }
        public async Task<ProfilesResponse> GetProfilesResponse(List<string> userIds)
        {
            var request = new ProfilesRequest
            {
                UserIds = userIds
            };

            var response = await _profileServiceClient.GetProfilesAsync(request);
            return response;
        }
    }
}
