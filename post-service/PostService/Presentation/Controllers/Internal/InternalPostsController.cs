using Application.DTOs;
using Application.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Internal
{
    [Route("api/internal/posts")]
    [ApiController]
    public class InternalPostsController : ControllerBase
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
            ServiceResponse<PostResponseDTO> res = await _postService.GetPostByIdAsync(postId);
            if (!res.IsValid)
            {
                return HandleServiceError(res);
            }
            return Ok(new { data = res.DataItem });
        }

        [HttpPost("user/{profileUserId}")]
        public async Task<IActionResult> GetProfilePostList(string profileUserId, [FromBody] GetProfilePostListRequest request)
        {
            // Encrypt the Key if exists
            if (!string.IsNullOrWhiteSpace(request.NextCursor))
                request.NextCursor = _encryptionService.Decrypt(request.NextCursor);

            const int pageSize = 15 + 1;
            ServiceResponse<PostResponseDTO> res = await _postService.GetProfilePostListAsync(request.UserId, profileUserId, pageSize, request.NextCursor);

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
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetPostList([FromBody] GetPostListRequest request)
        {
            ServiceResponse<PostResponseDTO> res = await _postService.GetPostListAsync(request.UserId, request.PostIds);

            if (!res.IsValid)
                return HandleServiceError(res);

            return Ok(new { data = res.DataList });
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
