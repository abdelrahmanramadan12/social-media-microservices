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
        private readonly IHelperService _helperService;
        public PostService(IPostRepository postRepository, IValidationService validationService, IHelperService helperService)
        {
            this._postRepository = postRepository;
            this._validationService = validationService;
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
            post.AuthorId = userId;
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
            MappingResult<PostResponseDTO> mappingResult = _helperService.MapPostToPostResponseDto(post);
            if (!mappingResult.Success)
            {
                res.Errors = mappingResult.Errors;
                res.ErrorType = mappingResult.ErrorType;
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

        public async Task<ServiceResponse<PostResponseDTO>> GetPostByIdAsync(string postId)
        {
            var response = new ServiceResponse<PostResponseDTO>();
            var postResponse = new PostResponseDTO();

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

            // Mapping -> PostResponseDto
            var mappingResult = _helperService.MapPostToPostResponseDto(post);
            if (!mappingResult.Success)
            {
                response.Errors = mappingResult.Errors;
                response.ErrorType = mappingResult.ErrorType;
                return response;
            }
            response.DataItem = mappingResult.Item;
            return response;
        }

        public async Task<ServiceResponse<PostResponseDTO>> GetProfilePostListAsync(string userId, string targetUserId, int pageSize, string cursorPostId)
        {
            var response = new ServiceResponse<PostResponseDTO>();

            // GET: fetch posts based on the privacy constrains
            List<Post> profilePosts = await _postRepository.GetUserPostsAsync(targetUserId, pageSize, cursorPostId);
            if (profilePosts == null || profilePosts.Count() <= 0)
            {
                response.DataList = new List<PostResponseDTO>();
                return response;
            }

            // Filter out OnlyMe posts where user is not the author
            profilePosts = profilePosts.Where(post =>
                post.Privacy != Privacy.OnlyMe || post.AuthorId == userId
            ).ToList();

            // Aggregate: 
            response.DataList = _helperService.AgregatePostResponseList(profilePosts);
            return response;
        }

        public async Task<ServiceResponse<PostResponseDTO>> GetPostListAsync(string userId, List<string> PostIds)
        {
            var response = new ServiceResponse<PostResponseDTO>();

            // Validate userId
            if (string.IsNullOrEmpty(userId))
            {
                response.ErrorType = ErrorType.UnAuthorized;
                response.Errors.Add("Invalid Request! Missing the User Id");
                return response;
            }

            response.DataList = new List<PostResponseDTO>();
            List<Post> reactedPosts = await _postRepository.GetPostList(userId, PostIds);
            if (reactedPosts == null || !reactedPosts.Any())
            {
                response.DataList = new List<PostResponseDTO>();
                return response;
            }

            // Map to DTOs
            response.DataList = _helperService.AgregatePostResponseList(reactedPosts);
            return response;
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

            var postResponseDto = _helperService.MapPostToPostResponseDto(postToUpdate);
            response.DataItem = postResponseDto.Item;
            return response!;
        }
    }
}