using Microsoft.AspNetCore.Mvc;
using react_service.Application.DTO.Reaction.Request;
using react_service.Application.DTO.Reaction.Request.Comment;
using react_service.Application.Interfaces.Services;
using Web.Controllers;

namespace react_service.Controllers.Public
{
    [ApiController]
    [Route("api/public/reacts/comment")]
    public class PublicReactionCommentController : BaseController
    {
        private readonly IReactionCommentService _reactionService;

        public PublicReactionCommentController(IReactionCommentService reactionService)
        {
            _reactionService = reactionService;
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteReaction([FromBody] DeleteCommentReactionRequest request, [FromHeader(Name = "userId")] string userId)
        {
            var res = await _reactionService.DeleteReactionAsync(request?.CommentId, userId);
            if (res.Success)
            {
                return HandleResponse(res);
            }
            else
            {
                return HandleErrorResponse(res);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddReaction([FromBody] CreateCommentReactionRequest request, [FromHeader(Name = "userId")] string userId)
        {
            var res = await _reactionService.AddReactionAsync(request, userId);
            if (res.Success)
            {
                return HandleResponse(res);
            }
            else
            {
                return HandleErrorResponse(res);
            }
        }
    }
}