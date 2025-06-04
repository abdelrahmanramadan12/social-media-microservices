using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces.CommentServices;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InternalCommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public InternalCommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> Get(
            [FromBody] CommentListRequest request)
        {
            var comments = await _commentService.ListCommentsAsync(request.PostId, request.Next);

            return Ok(new {data = comments.Comments, next = comments.Next});
        }

        [HttpPost("details")]
        public async Task<IActionResult> Get(
            [FromBody] string commentId)
        {
            var comment = await _commentService.GetCommentAsync(commentId);

            return Ok(comment);
        }


    }
}
