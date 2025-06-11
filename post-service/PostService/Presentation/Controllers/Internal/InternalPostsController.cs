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

        [HttpPost("user")]
        public async Task<IActionResult> GetProfilePostList([FromBody] GetProfilePostListRequest request)
        {
            string decryptedCursor = null!;
            if (!string.IsNullOrWhiteSpace(request.Next))
            {
                try
                {
                    decryptedCursor = _encryptionService.Decrypt(request.Next);
                }
                catch
                {
                    decryptedCursor = null!;
                }
            }

            const int pageSize = 15;
            var res = await _postService.GetProfilePostListAsync(request.UserId, request.ProfileUserId, pageSize, decryptedCursor);

            if (!res.Success)
                return HandleErrorResponse(res);

            if (res.Data == null || !res.Data.Any())
            {
                return Ok(new { data = new List<object>(), next = (string?)null, message = res.Message ?? "No posts found" });
            }

            var formattedList = res.Data.Select(post => FormatPostData(post)).ToList();
            string nextCursor = null;

            // If we got exactly pageSize items, there might be more posts
            if (res.Data.Count == pageSize)
            {
                var lastPost = res.Data[^1];
                try
                {
                    nextCursor = _encryptionService.Encrypt(lastPost.PostId);
                }
                catch
                {
                    // If encryption fails, treat it as if there are no more posts
                    nextCursor = null;
                }
            }

            return Ok(new { data = formattedList, next = nextCursor, message = res.Message ?? "Posts retrieved successfully" });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetPostList([FromBody] GetPostListRequest request)
        {
            var res = await _postService.GetPostListAsync(request.UserId, request.PostIds);
            return HandleResponse(res);
        }
    }
}
