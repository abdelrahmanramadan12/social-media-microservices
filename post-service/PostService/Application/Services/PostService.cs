using Application.DTOs;
using Application.IServices;
using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using Domain.ValueObjects;

namespace Application.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IValidationService _validationService;
        private readonly IMediaServiceClient _mediaServiceClient;
        private readonly IHelperService _helperService;
        public PostService(IPostRepository postRepository, IValidationService validationService, IMediaServiceClient mediaServiceClient, IHelperService helperService)
        {
            this._postRepository = postRepository;
            this._validationService = validationService;
            this._mediaServiceClient = mediaServiceClient;
            _helperService = helperService;
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
                var uploadResponse = await _helperService.AssignMediaToPostInput(postInputDto);
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
            MappingResult<PostResponseDTO> mappingResult = await _helperService.MapPostToPostResponseDto(post, post.AuthorId, false, true);
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
            var accessResult = await _helperService.CheckPostAccess(userId, post);
            if (!accessResult.IsValid)
            {
                response.Errors = accessResult.Errors;
                response.ErrorType = accessResult.ErrorType;
                return response;
            }

            // Mapping -> PostResponseDto
            // IsLiked ??
            // Assign the author profile

            var mappingResult = await _helperService.MapPostToPostResponseDto(post, userId, true, true);
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

        public async Task<ServiceResponse<PostResponseDTO>> UpdatePostAsync(string userId, PostInputDTO postInputDto)
        {
            var response = new ServiceResponse<PostResponseDTO>();
            var postToUpdate = await _postRepository.GetPostAsync(postInputDto.PostId);

            // Validate Post Update (Including security)
            var validationResult = await _validationService.ValidateUpdatePost(postInputDto, postToUpdate, userId);
            if (!validationResult.IsValid)
            {
                response.Errors = validationResult.Errors;
                response.ErrorType = validationResult.ErrorType;
                return response;
            }

            // Update Data (Locally)
            postToUpdate.Content = postInputDto.Content;
            postToUpdate.Privacy = postInputDto.Privacy;

            // Update Media
            var updateMediaResponse = await _helperService.UpdatePostMedia(postInputDto, postToUpdate);
            if (!updateMediaResponse.IsValid)
            {
                response.Errors = updateMediaResponse.Errors;
                response.ErrorType = updateMediaResponse.ErrorType;
                return response;
            }
            postToUpdate = updateMediaResponse.DataItem;

            // Add to the DB
            var updateResult = await _postRepository.UpdatePostAsync(postToUpdate.Id, postToUpdate, postInputDto.HasMedia);

            var postResponseDto = await _helperService.MapPostToPostResponseDto(postToUpdate, postToUpdate.AuthorId, true, true);
            response.DataItem = postResponseDto.Item;
            return response!;
        }


    }
}