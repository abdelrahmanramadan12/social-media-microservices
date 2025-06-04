using Domain.IRepository;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces.CommentServices;

namespace Web.Controllers.Internal
{
    [Route("api/internal/comment")]
    [ApiController]
    public class InternalCommentController : BaseController
    {
        private readonly ICommentService _commentService;
        public InternalCommentController(ICommentRepository commentRepository, ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> Get(
            [FromRoute] string postId,
            [FromQuery] string? next = null)
        {
            var response = await _commentService.ListCommentsAsync(postId, next);
            return HandlePaginatedResponse(response);
        }
    }
}