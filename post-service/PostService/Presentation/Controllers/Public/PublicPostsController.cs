using Application.DTOs;
using Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Public
{
    [Route("api/public/posts")]
    [ApiController]
    public class PublicPostsController : ControllerBase
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
        public async Task<IActionResult> DeletePost([FromBody] DeletePostRequest request, [FromHeader(Name = "userId")] string userId)
        {
            var res = await _postService.DeletePostAsync(userId, request.PostId);
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
