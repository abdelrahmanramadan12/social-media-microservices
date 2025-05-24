using Application.DTOs;
using Application.IServices;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Presentation.Controllers.Public
{
    [Route("api/public/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IEncryptionService _encryptionService;
        public PostsController(IPostService postSerive, IEncryptionService encryptionService)
        {
            this._postService = postSerive;
            this._encryptionService = encryptionService;
        }


        // Endpoints

        [HttpGet("{postId}")]
        public async Task<IActionResult> GetPost(string postId, [FromHeader(Name = "userId")] string userId)
        {
            ServiceResponse<PostResponseDTO> res = await _postService.GetPostByIdAsync(userId, postId);
            if (!res.IsValid)
            {
                return HandleServiceError(res);
            }
            return Ok(new { data = res.DataItem });
        }

        [HttpGet("user/{profileUserId}")]
        public async Task<IActionResult> GetProfilePostList(string profileUserId, [FromHeader(Name = "userId")] string userId, [FromQuery] string? next)
        {
            // Encrypt the Key if exists
            if (!string.IsNullOrWhiteSpace(next))
                next = _encryptionService.Decrypt(next);

            const int pageSize = 15 + 1;
            ServiceResponse<PostResponseDTO> res = await _postService.GetProfilePostListAsync(userId ,profileUserId, pageSize, next);
            
            if (!res.IsValid)
                return HandleServiceError(res);

            if (res.DataList?.Count >= pageSize)
            {
                var lastPost = res.DataList[^1];
                res.DataList.RemoveAt(res.DataList.Count - 1);
                string nextPostIdEncrpted = _encryptionService.Encrypt(lastPost.PostId);
                return Ok(new { data = res.DataList , next = nextPostIdEncrpted});                       
            }

            return Ok(new { data = res.DataList, next = (string?)null });
        }

        [HttpGet("reacted")]
        public async Task<IActionResult> GetReactedPostList([FromHeader(Name = "userId")] string userId, [FromQuery] string? next)
        {
            // Encrypt the Key if exists
            if (!string.IsNullOrWhiteSpace(next))
                next = _encryptionService.Decrypt(next);

            const int pageSize = 15 + 1;
            ServiceResponse<PostResponseDTO> res = await _postService.GetReactedPostListAsync(userId, pageSize, next);

            if (!res.IsValid)
                return HandleServiceError(res);

            if (res.DataList?.Count >= pageSize)
            {
                var lastPost = res.DataList[^1];
                res.DataList.RemoveAt(res.DataList.Count - 1);
                string nextPostIdEncrpted = _encryptionService.Encrypt(lastPost.PostId);
                return Ok(new { data = res.DataList, next = nextPostIdEncrpted });
            }

            return Ok(new { data = res.DataList, next = (string?)null });


            return null!;
        }

        [HttpPost]
        public async Task<IActionResult> AddPost([FromForm] PostInputDTO postDto, [FromHeader(Name = "userId")] string userId)
        {
            ServiceResponse<PostResponseDTO> res = await _postService.AddPostAsync(userId, postDto);
            if (!res.IsValid)
            {
                return HandleServiceError(res);
            }

            return CreatedAtAction("Create", new { res.DataItem });
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePost([FromForm] PostInputDTO postDto, [FromHeader(Name = "userId")] string userId)
        {
            ServiceResponse<PostResponseDTO> res = await _postService.UpdatePostAsync(userId, postDto);
            if (!res.IsValid)
            {
                return HandleServiceError(res);
            }

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePost([FromBody] string postId, [FromHeader(Name = "userId")] string userId)
        {
            var res = await _postService.DeletePostAsync(userId ,postId);
            if (!res.IsValid)
                return HandleServiceError(res);
            
            return NoContent();
        }
        

        // Utilities
        private ActionResult HandleServiceError<T>(ServiceResponse<T> res)
        {
            return res.ErrorType switch
            {
                ErrorType.NotFound => NotFound(new { errors = res.Errors }),
                ErrorType.BadRequest => BadRequest(new { errors = res.Errors }),
                ErrorType.UnAuthorized => Unauthorized(new { errors = res.Errors }),
                ErrorType.InternalServerError => StatusCode(StatusCodes.Status500InternalServerError, res.Errors),
                _ => BadRequest(new { errors = res.Errors }),
            };
        }
    }
}
