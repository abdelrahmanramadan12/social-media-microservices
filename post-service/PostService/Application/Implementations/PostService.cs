using Application.DTOs;
using Application.DTOs.Responses;
using Application.Events;
using Application.Interfaces;
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
        private readonly IQueuePublisher<PostEvent> _queuePublisher;
        public PostService(IPostRepository postRepository, IValidationService validationService, IHelperService helperService, IQueuePublisher<PostEvent> queuePublisher)
        {
            this._postRepository = postRepository;
            this._validationService = validationService;
            _helperService = helperService;
            _queuePublisher = queuePublisher;
        }

        public async Task<ResponseWrapper<PostResponseDTO>> AddPostAsync(string userId, PostInputDTO postInputDto)
        {
            var res = new ResponseWrapper<PostResponseDTO>();
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
            res.Data = mappingResult.Item;
            res.Message = "Post created successfully";
            var postCreatedEvent = new PostEvent();
            postCreatedEvent.EventType = EventType.Create;

            // Mapping 
            postCreatedEvent.AuthorId = res.Data.AuthorId;
            postCreatedEvent.AuthorId = res.Data.AuthorId;
            postCreatedEvent.PostId = res.Data.PostId;
            postCreatedEvent.PostContent = res.Data.PostContent;
            postCreatedEvent.Privacy = res.Data.Privacy;
            postCreatedEvent.Media = res.Data.Media;
            postCreatedEvent.CreatedAt = res.Data.CreatedAt;
            postCreatedEvent.IsEdited = res.Data.IsEdited;
            postCreatedEvent.NumberOfLikes = res.Data.NumberOfLikes;
            postCreatedEvent.NumberOfComments = res.Data.NumberOfComments;
            
            await _queuePublisher.PublishAsync(postCreatedEvent);
            return res;
        }

        public async Task<ResponseWrapper<string>> DeletePostAsync(string userId, string postId)
        {
            var response = new ResponseWrapper<string>();
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

            // Publish Delete Event
            var postDeletedEvent = new PostEvent
            {
                EventType = EventType.Delete,
                PostId = postId 
            };
            await _queuePublisher.PublishAsync(postDeletedEvent);

            response.Data = postId;
            response.Message = "Post deleted successfully";
            return response;
        }

        public async Task<ResponseWrapper<PostResponseDTO>> GetPostByIdAsync(string postId)
        {
            var response = new ResponseWrapper<PostResponseDTO>();
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
            response.Data = mappingResult.Item;
            response.Message = "Post retrieved successfully";
            return response;
        }

        public async Task<ResponseWrapper<List<PostResponseDTO>>> GetProfilePostListAsync(string userId, string targetUserId, int pageSize, string cursorPostId)
        {
            var response = new ResponseWrapper<List<PostResponseDTO>>();
            List<Post> profilePosts = await _postRepository.GetUserPostsAsync(targetUserId, pageSize, cursorPostId);
            if (profilePosts == null || profilePosts.Count() <= 0)
            {
                response.Data = new List<PostResponseDTO>();
                response.Message = "No posts found";
                return response;
            }
            profilePosts = profilePosts.Where(post => post.Privacy != Privacy.OnlyMe || post.AuthorId == userId).ToList();
            response.Data = _helperService.AgregatePostResponseList(profilePosts);
            response.Message = "Posts retrieved successfully";
            return response;
        }

        public async Task<ResponseWrapper<List<PostResponseDTO>>> GetPostListAsync(string userId, List<string> PostIds)
        {
            var response = new ResponseWrapper<List<PostResponseDTO>>();
            if (string.IsNullOrEmpty(userId))
            {
                response.ErrorType = ErrorType.UnAuthorized;
                response.Errors.Add("Invalid Request! Missing the User Id");
                return response;
            }
            List<Post> reactedPosts = await _postRepository.GetPostList(userId, PostIds);
            if (reactedPosts == null || !reactedPosts.Any())
            {
                response.Data = new List<PostResponseDTO>();
                return response;
            }
            response.Data = _helperService.AgregatePostResponseList(reactedPosts);
            return response;
        }

        public async Task<ResponseWrapper<PostResponseDTO>> UpdatePostAsync(string userId, PostInputDTO postInputDto)
        {
            var response = new ResponseWrapper<PostResponseDTO>();
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
            Console.WriteLine(postInputDto.MediaUrls.Count());
            var updateMediaResponse = await _helperService.UpdatePostMedia(postInputDto, postToUpdate);
            if (!updateMediaResponse.Success)
            {
                response.Errors = updateMediaResponse.Errors;
                response.ErrorType = updateMediaResponse.ErrorType;
                return response;
            }

            // Save changes
            var updatedPost = await _postRepository.UpdatePostAsync(postToUpdate.Id, postToUpdate, postInputDto.HasMedia);
            if (updatedPost == null)
            {
                response.ErrorType = ErrorType.InternalServerError;
                response.Errors.Add("Failed to update the post in the database.");
                return response;
            }

            // Map to DTO
            var mappingResult = _helperService.MapPostToPostResponseDto(updatedPost);
            if (!mappingResult.Success)
            {
                response.Errors = mappingResult.Errors;
                response.ErrorType = mappingResult.ErrorType;
                return response;
            }
            response.Data = mappingResult.Item;
            response.Message = "Post updated successfully";

            // Publish Update Event
            var postUpdatedEvent = new PostEvent
            {
                EventType = EventType.Update,
                AuthorId = mappingResult.Item.AuthorId,
                PostId = mappingResult.Item.PostId,
                PostContent = mappingResult.Item.PostContent,
                Privacy = mappingResult.Item.Privacy,
                Media = mappingResult.Item.Media,
                CreatedAt = mappingResult.Item.CreatedAt,
                IsEdited = mappingResult.Item.IsEdited,
                NumberOfLikes = mappingResult.Item.NumberOfLikes,
                NumberOfComments = mappingResult.Item.NumberOfComments
            };
            await _queuePublisher.PublishAsync(postUpdatedEvent);

            return response;
        }
    }
}