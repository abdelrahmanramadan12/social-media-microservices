using Domain.IRepository;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Requests;
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

        [HttpPost("list")]
        public async Task<IActionResult> Get([FromBody] GetPagedCommentRequest request)
        {
            var response = await _commentService.ListCommentsAsync(request);
            return HandlePaginatedResponse(response);
        }
    }
}