using Application.DTOs;
using Application.IServices;
using Domain.Entities;
using Domain.IRepository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IValidationService _validationService;
        public PostService(IPostRepository postRepository, IValidationService validationService)
        {
            this._postRepository = postRepository;
            this._validationService = validationService;
        }

        public async Task<ServiceResponse<PostResponseDTO>> AddPostAsync(string userId, PostDTO postDto)
        {
            var res = new ServiceResponse<PostResponseDTO>();

            // Validate
            var validationResult = await _validationService.ValidateNewPost(postDto, userId);
            if (!validationResult.IsValid)
            {
                res.Errors = validationResult.Errors;
                res.ErrorType = res.ErrorType;
                return res;
            }else
            {
                postDto.userId = userId;
            }

            // Upload Media 
            if (postDto.HasMedia)
            {
                // Media Service 
                // Assign Return URL
            }

            // Add to the DB 
            Post newPost = new Post();

            var post = await _postRepository.CreatePostAsync(newPost, postDto.HasMedia);
            if (post != null)
            {
                var postResponse  = MapPostToPostResponseDto(post);
                res.DataItem = postResponse;
            }else
            {
                res.Errors.Add("Faild to add the post to the DB");
            }

            return res;
        }

        public Task<bool> DeletePostAsync(string userId, string postId, out string message)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<PostResponseDTO>> GetPostByIdAsync(string userId, string postId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<PostResponseDTO>> GetProfilePostListAsync(string userId, string profileUserId, int pageSize, string cursorPostId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<PostResponseDTO>> GetReactedPostListAsync(string userId, int pageSize, string cursorPostId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<PostResponseDTO>> UpdatePostAsync(string userId, PostDTO postDto)
        {
            throw new NotImplementedException();
        }

        private Post MapPostDtoToPost(PostDTO postDto)
        {
            Post post = new Post()
            {
                Privacy = postDto.Privacy,
                Content = postDto.Content,
                AuthorId = postDto.userId,
            };

            return post;

        }
        private PostResponseDTO MapPostToPostResponseDto(Post post)
        {
            var postResponseDto = new PostResponseDTO()
            {
                PostId = post.Id,
                PostContent = post.Content,
                CreatedAt = post.CreatedAt,
                IsEdited = false,
                IsLiked = false,
                AuthorId = post.AuthorId,
                MediaURL = post.MediaList.Select(ml => ml.MediaUrl).ToList(),
                NumberOfComments = post.NumberOfComments,
                NumberOfLikes = post.NumberOfLikes,
            };

            return postResponseDto;
        }
    }
}