using Microsoft.AspNetCore.Mvc;
using reat_service.Application.DTO.ReactionPost.Request;
using reat_service.Application.Interfaces.Services;

namespace react_service.Controllers
{
    [ApiController]
    [Route("api/public/reacts/post")]
    public class ReactionPostController : ControllerBase
    {
        private readonly IReactionPostService _reactionService;

        public ReactionPostController(IReactionPostService reactionService) 
        {
            _reactionService = reactionService;
        }

        [HttpGet("~/api/public/reacts/post/{postId}")]

        public async Task<IActionResult> GetReactsByPost(  string postId,[FromQuery] string? nextReactIdHash , [FromHeader(Name = "userId")] string userId)
        {

            if (string.IsNullOrEmpty(postId))
            {
                return BadRequest("Post ID cannot be null or empty.");
            }
            var result = await _reactionService.GetReactsByPostAsync(postId,nextReactIdHash , userId);
            return Ok(result);
        }

        [HttpDelete("~/api/public/reacts/")]

        public async Task<IActionResult> DeleteReaction([FromBody] DeleteReactionRequest request, [FromHeader(Name = "userId")] string userId)
        {
            if (request == null || string.IsNullOrEmpty(request.PostId))
            {
                return BadRequest(new { postId = request?.PostId ?? string.Empty });
            }
            var res =  await _reactionService.DeleteReactionAsync(request.PostId, userId);
            if(res == false)
            {
                return NotFound(new { message = "Reaction not found.", postId = request?.PostId });
            }
            return NoContent(); 

        }

        [HttpPost("~/api/public/reacts/")]
        public async Task<IActionResult> AddReaction([FromForm] CreateReactionRequest request, [FromHeader(Name = "userId")] string userId)
        {
            if (string.IsNullOrEmpty(request.PostId))
            {
                return BadRequest("Post ID cannot be null or empty.");
            }
           
            await _reactionService.AddReactionAsync(request , userId);
            return NoContent();
        }

    }
}
