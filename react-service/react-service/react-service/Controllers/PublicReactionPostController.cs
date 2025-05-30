using Microsoft.AspNetCore.Mvc;
using react_service.Application.DTO.ReactionPost.Request;
using react_service.Application.Interfaces.Services;

namespace react_service.Controllers
{
    [ApiController]
    [Route("api/public/reacts/post")]
    public class PublicReactionPostController : ControllerBase
    {
        private readonly IReactionPostService _reactionService;

        public PublicReactionPostController(IReactionPostService reactionService) 
        {
            _reactionService = reactionService;
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteReaction([FromBody] DeleteReactionRequest request, [FromHeader(Name = "userId")] string userId)
        {
            if (request == null || string.IsNullOrEmpty(request.PostId))
            {
                return BadRequest(new { postId = request?.PostId ?? string.Empty });
            }
            var res = await _reactionService.DeleteReactionAsync(request.PostId, userId);
            if(res == false)
            {
                return NotFound(new { message = "Reaction not found.", postId = request?.PostId });
            }
            return NoContent(); 
        }

        [HttpPost]
        public async Task<IActionResult> AddReaction([FromForm] CreateReactionRequest request, [FromHeader(Name = "userId")] string userId)
        {
            if (string.IsNullOrEmpty(request.PostId))
            {
                return BadRequest("Post ID cannot be null or empty.");
            }
           
            await _reactionService.AddReactionAsync(request, userId);
            return NoContent();
        }
    }
}