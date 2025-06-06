using Application.DTOs;
using Application.DTOs.Responses;
using Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Public
{
    [Route("api/public/posts")]
    [ApiController]
    public class PublicPostsController : BaseController
    {
        private readonly IPostService _postService;
        private readonly IEncryptionService _encryptionService;
        public PublicPostsController(IPostService postSerive, IEncryptionService encryptionService)
        {
            this._postService = postSerive;
            this._encryptionService = encryptionService;
        }

        [HttpPost]
        public async Task<IActionResult> AddPost([FromForm] PostInputDTO postDto, [FromHeader(Name = "userId")] string userId)
        {
            var res = await _postService.AddPostAsync(userId, postDto);
            if (!res.Success)
                return HandleErrorResponse(res);
            var responseData = res.Data;
            if (responseData == null)
                return StatusCode(500, new { errors = new[] { "Unexpected error: response data is null." } });
            var mediaWithType = new List<object>();
            if (responseData.MediaUrls != null)
            {
                foreach (var url in responseData.MediaUrls)
                {
                    mediaWithType.Add(new { url, mediaType = postDto.MediaType.ToString() });
                }
            }
            var response = new
            {
                data = new {
                    responseData.PostId,
                    responseData.AuthorId,
                    responseData.PostContent,
                    responseData.Privacy,
                    Media = mediaWithType,
                    responseData.HasMedia,
                    responseData.CreatedAt,
                    responseData.IsEdited,
                    responseData.NumberOfLikes,
                    responseData.NumberOfComments
                },
                message = "Post created successfully",
                success = res.Success,
                errorType = res.ErrorType
            };
            return Created($"api/public/posts/{responseData.PostId}", response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePost([FromForm] PostInputDTO postDto, [FromHeader(Name = "userId")] string userId)
        {
            var res = await _postService.UpdatePostAsync(userId, postDto);
            if (!res.Success)
                return HandleErrorResponse(res);
            var responseData = res.Data;
            if (responseData == null)
                return StatusCode(500, new { errors = new[] { "Unexpected error: response data is null." } });
            var mediaWithType = new List<object>();
            if (responseData.MediaUrls != null)
            {
                foreach (var url in responseData.MediaUrls)
                {
                    mediaWithType.Add(new { url, mediaType = postDto.MediaType.ToString() });
                }
            }
            var response = new
            {
                data = new {
                    responseData.PostId,
                    responseData.AuthorId,
                    responseData.PostContent,
                    responseData.Privacy,
                    Media = mediaWithType,
                    responseData.HasMedia,
                    responseData.CreatedAt,
                    responseData.IsEdited,
                    responseData.NumberOfLikes,
                    responseData.NumberOfComments
                },
                message = "Post updated successfully",
                success = res.Success,
                errorType = res.ErrorType
            };
            return Ok(response);
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePost([FromBody] DeletePostRequest request, [FromHeader(Name = "userId")] string userId)
        {
            var res = await _postService.DeletePostAsync(userId, request.PostId);
            if (!res.Success)
                return HandleErrorResponse(res);
            return NoContent();
        }
    }
}
