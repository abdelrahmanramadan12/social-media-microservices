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
            if (!string.IsNullOrWhiteSpace(request.Next))
                request.Next = _encryptionService.Decrypt(request.Next);

            const int pageSize = 15 + 1;
            var res = await _postService.GetProfilePostListAsync(request.UserId, profileUserId, pageSize, request.Next);

            if (!res.Success)
                return HandleErrorResponse(res);

            var formattedList = res.Data.Select(post => new
            {
                post.PostId,
                post.AuthorId,
                post.PostContent,
                post.Privacy,
                media = post.Media?.Select(m => new { url = m.Url, type = (int)m.Type }).ToList(),
                post.HasMedia,
                post.CreatedAt,
                post.IsEdited,
                post.NumberOfLikes,
                post.NumberOfComments
            }).ToList();

            if (res.Data?.Count >= pageSize)
            {
                var lastPost = res.Data[^1];
                formattedList.RemoveAt(formattedList.Count - 1);
                string nextPostIdEncrpted = _encryptionService.Encrypt(lastPost.PostId);
                return Ok(new { data = formattedList, next = nextPostIdEncrpted, message = res.Message });
            }

            return Ok(new { data = formattedList, next = (string?)null, message = res.Message });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetPostList([FromBody] GetPostListRequest request)
        {
            var res = await _postService.GetPostListAsync(request.UserId, request.PostIds);
            return HandleResponse(res);
        }
    }
}
