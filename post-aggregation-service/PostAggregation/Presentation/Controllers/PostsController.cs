using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/public/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostAggregationService _postAggregationService;
        public PostsController(IPostAggregationService postAggregationService)
        {
            this._postAggregationService = postAggregationService;
        }

        [HttpGet("user/posts/{targetUserId}")]
        public async Task<IActionResult> GetProfilePosts(string targetUserId, [FromHeader(Name = "userId")] string userId, [FromQuery] string? next)
        {
            var response = await _postAggregationService.GetProfilePosts(targetUserId, userId, next);
            if (response.Success)
            {
                return Ok(new {data = response.ItemList, next = response.NextPostHashId});
            }
            return BadRequest();
        }
    }
}
