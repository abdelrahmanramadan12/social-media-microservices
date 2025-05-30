using Microsoft.AspNetCore.Mvc;
using react_service.Application.DTO.ReactionPost.Request;
using react_service.Application.Interfaces.Services;

namespace react_service.Controllers
{
    [ApiController]
    [Route("api/internal/reacts/post")]
    public class InternalReactionPostController : ControllerBase
    {
        private readonly IReactionPostService _reactionService;

        public InternalReactionPostController(IReactionPostService reactionService) 
        {
            _reactionService = reactionService;
        }

        [HttpPost("user/reacted")]
        public async Task<IActionResult> GetPostsReactedByUser([FromBody] GetPostsReactedByUserRequest request)
        {
            if (request?.PostIds == null || request.PostIds.Count == 0)
            {
                return BadRequest("Post IDs cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }
            var reactedPosts = await _reactionService.IsPostsReactedByUserAsync(request.PostIds, request.UserId);
            return Ok(reactedPosts);
        }   
    }
}