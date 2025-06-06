using Application.DTOs;
using Application.DTOs.Responses;
using Application.IServices;
using Domain.Entities;
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

            return Created($"api/public/posts/{res.Data.PostId}", new { data = FormatPostData(res.Data), message = res.Message });
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePost([FromForm] PostInputDTO postDto, [FromHeader(Name = "userId")] string userId)
        {
            var res = await _postService.UpdatePostAsync(userId, postDto);
            return HandleResponse(res);
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
