using Application.DTOs;
using Application.DTOs.Responses;
using Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Internal
{
    [Route("api/internal/posts")]
    [ApiController]
    public class InternalPostsController : BaseController
    {
        private readonly IPostService _postService;
        private readonly IEncryptionService _encryptionService;
        public InternalPostsController(IPostService postSerive, IEncryptionService encryptionService)
        {
            this._postService = postSerive;
            this._encryptionService = encryptionService;
        }
        // Endpoints
        [HttpGet("{postId}")]
        public async Task<IActionResult> GetPost(string postId)
        {
            var res = await _postService.GetPostByIdAsync(postId);
            return HandleResponse(res);
        }

        [HttpPost("user/{profileUserId}")]
        public async Task<IActionResult> GetProfilePostList(string profileUserId, [FromBody] GetProfilePostListRequest request)
        {
            // Encrypt the Key if exists
            if (!string.IsNullOrWhiteSpace(request.NextCursor))
                request.NextCursor = _encryptionService.Decrypt(request.NextCursor);

            const int pageSize = 15 + 1;
            var res = await _postService.GetProfilePostListAsync(request.UserId, profileUserId, pageSize, request.NextCursor);

            if (!res.Success)
                return HandleErrorResponse(res);

            if (res.Data?.Count >= pageSize)
            {
                var lastPost = res.Data[^1];
                res.Data.RemoveAt(res.Data.Count - 1);
                string nextPostIdEncrpted = _encryptionService.Encrypt(lastPost.PostId);
                return Ok(new { data = res.Data, next = nextPostIdEncrpted });
            }

            return Ok(new { data = res.Data, next = (string?)null });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetPostList([FromBody] GetPostListRequest request)
        {
            var res = await _postService.GetPostListAsync(request.UserId, request.PostIds);

            return HandleResponse(res);
        }
    }
}
