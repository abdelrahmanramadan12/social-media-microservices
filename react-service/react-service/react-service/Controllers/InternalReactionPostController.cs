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

        [HttpPost("user/filter")]
        public async Task<IActionResult> FilterPostsReactedByUser([FromBody] GetPostsReactedByUserRequest request)
        {
            if (request?.PostIds == null || request.PostIds.Count == 0)
            {
                return BadRequest("Post IDs cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }
            var reactedPosts = await _reactionService.FilterPostsReactedByUserAsync(request.PostIds, request.UserId);
            return Ok(reactedPosts);
        }   

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPostsReactedByUser(string userId, [FromQuery] string? nextReactIdHash)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }
            var result = await _reactionService.GetPostsReactedByUserAsync(userId, nextReactIdHash);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetReactsOfPost([FromBody] GetReactsOfPostRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.PostId))
            {
                return BadRequest("Post ID cannot be null or empty.");
            }
            var result = await _reactionService.GetReactsOfPostAsync(request.PostId, request.NextReactIdHash);
            return Ok(result);
        }

        
    }
}