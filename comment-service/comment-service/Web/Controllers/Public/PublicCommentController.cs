using Domain.IRepository;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.Requests;
using Service.Interfaces.CommentServices;

namespace Web.Controllers.Public
{
    [Route("api/public/comment")]
    [ApiController]
    public class PublicCommentController : BaseController
    {
        private readonly ICommentService _commentService;
        public PublicCommentController(ICommentRepository commentRepository, ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromForm] CreateCommentRequest createCommentRequestDto,
            [FromHeader(Name = "userId")] string userId)
        {
            createCommentRequestDto.UserId = userId;
            var response = await _commentService.CreateCommentAsync(createCommentRequestDto);
            return HandleCreatedResponse(response);
        }

        [HttpPut]
        public async Task<IActionResult> EditComment(
            [FromForm] EditCommentRequest editCommentRequestDto,
            [FromHeader(Name = "userId")] string userId)
        {
            editCommentRequestDto.UserId = userId;
            var response = await _commentService.UpdateCommentAsync(editCommentRequestDto);
            return HandleResponse(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, [FromHeader(Name = "userId")] string userId)
        {
            var response = await _commentService.DeleteCommentAsync(id, userId);
            return HandleNoContentResponse(response);
        }
    }
}
