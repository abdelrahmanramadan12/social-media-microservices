using Application.DTOs;
using Application.IServices;
using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using Domain.ValueObjects;
using ZstdSharp.Unsafe;

namespace Application.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IValidationService _validationService;
        private readonly IMediaServiceClient _mediaServiceClient;
        public PostService(IPostRepository postRepository, IValidationService validationService, IMediaServiceClient mediaServiceClient)
        {
            this._postRepository = postRepository;
            this._validationService = validationService;
            this._mediaServiceClient = mediaServiceClient;
        }

        public async Task<ServiceResponse<PostResponseDTO>> AddPostAsync(string userId, PostInputDTO postInputDto)
        {
            var res = new ServiceResponse<PostResponseDTO>();
            var post = new Post();

            // Validate Post Content
            var validationResult = await _validationService.ValidateNewPost(postInputDto, userId);
            if (!validationResult.IsValid)
            {
                res.Errors = validationResult.Errors;
                res.ErrorType = validationResult.ErrorType;
                return res;
            }
            post.AuthorId = postInputDto.AuthorId;
            post.Content = postInputDto.Content;
            post.Privacy = postInputDto.Privacy;


            // Upload Media 
            if (postInputDto.HasMedia)
            {
                var uploadResponse = await AssignMediaToPostInput(postInputDto);
                if (uploadResponse.Success)
                {
                    post.MediaList = uploadResponse.Urls
                        .Select(url => new Media
                        {
                            MediaUrl = url,
                            MediaType = postInputDto.MediaType
                        })
                        .ToList();
                }
                else
                {
                    res.Errors.Add("Falied to upload media!");
                    res.ErrorType = ErrorType.BadRequest;
                    return res;
                }
            }

            // Add to the DB 
            post = await _postRepository.CreatePostAsync(post, postInputDto.HasMedia);
            if (post == null)
            {
                res.Errors.Add("Faild to add the post to the DB");
            }

            // Map Post => PostResponse
            MappingResult<PostResponseDTO> mappingResult = await MapPostToPostResponseDto(post, post.AuthorId, false, true);
            if (!mappingResult.Success)
            {
                res.Errors = mappingResult.Errors;
                return res;
            }
            res.DataItem = mappingResult.Item;

            return res;
        }

        public async Task<ServiceResponse<string>> DeletePostAsync(string userId, string postId)
        {
            var response = new ServiceResponse<string>();
            if (string.IsNullOrEmpty(postId))
            {
                response.ErrorType = ErrorType.BadRequest;
                response.Errors.Add("Invalid Request! Missing the Post Id");
                return response;
            }
            if (string.IsNullOrEmpty(userId))
            {
                response.ErrorType = ErrorType.UnAuthorized;
                response.Errors.Add("Invalid Request! Missing the User Id");
                return response;
            }
            var result = await _postRepository.DeletePostAsync(postId, userId);
            if (!result)
            {
                response.ErrorType = ErrorType.BadRequest;
                response.Errors.Add("Invalid Rperation! post isn't found or you don't have permession");
                return response;
            }
            response.DataItem = postId;
            return response;
        }

        public async Task<ServiceResponse<PostResponseDTO>> GetPostByIdAsync(string userId, string postId)
        {
            var response = new ServiceResponse<PostResponseDTO>();
            var postResponse = new PostResponseDTO();

            if (string.IsNullOrEmpty(userId))
            {
                response.ErrorType = ErrorType.BadRequest;
                response.Errors.Add($"Invalid Request! You are not authorized");
                return response;
            }

            if (string.IsNullOrEmpty(postId))
            {
                response.ErrorType = ErrorType.BadRequest;
                response.Errors.Add($"Invalid Request! Post {postId} doesn't or it has been deleted!");
                return response;
            }
            var post = await _postRepository.GetPostAsync(postId);
            if (post == null)
            {
                response.ErrorType = ErrorType.BadRequest;
                response.Errors.Add($"Invalid Request! Post {postId} doesn't or it has been deleted!");
                return response;
            }
            var accessResult = await CheckPostAccess(userId, post);
            if (!accessResult.IsValid)
            {
                response.Errors = accessResult.Errors;
                response.ErrorType = accessResult.ErrorType;
                return response;
            }

            // Mapping -> PostResponseDto
            // IsLiked ??
            // Assign the author profile

            var mappingResult = await MapPostToPostResponseDto(post, userId, true, true);
            if (!mappingResult.Success)
            {
                response.Errors = mappingResult.Errors;
                response.ErrorType = mappingResult.ErrorType;
                return response;
            }
            response.DataItem = mappingResult.Item;
            return response;
        }

        public Task<ServiceResponse<PostResponseDTO>> GetProfilePostListAsync(string userId, string profileUserId, int pageSize, string cursorPostId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<PostResponseDTO>> GetReactedPostListAsync(string userId, int pageSize, string cursorPostId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<PostResponseDTO>> UpdatePostAsync(string userId, PostInputDTO postInputDto)
        {
            throw new NotImplementedException();
        }


        // Helper Methods
        private async Task<MappingResult<PostResponseDTO>> MapPostToPostResponseDto(Post post, string userId,bool checkIsLiked, bool assignProfile)
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
        private async Task<MediaUploadResponse> AssignMediaToPostInput(PostInputDTO postInputDTO)
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
        private async Task<ValidationResult> CheckPostAccess(string userId, Post post)
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

            }catch
            {
                result.Errors.Add("Something wen't wrong while checking your perrmession. try again later!");
                result.ErrorType = ErrorType.InternalServerError;
                return result;
            }
        }
    }
}