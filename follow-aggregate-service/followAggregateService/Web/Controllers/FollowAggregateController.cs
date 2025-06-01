using Application.Abstractions;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/public/follows-aggregate")]
    [ApiController]
    public class FollowAggregateController : ControllerBase
    {
        private readonly IFollowQueryService _followQueryService;

        public FollowAggregateController(IFollowQueryService followService)
        {
            _followQueryService = followService;
        }

        [HttpGet("list-following-page/{userId}")]
        [ProducesResponseType(typeof(FollowsPageDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListFollowingProfiles(string userId, [FromQuery] string? next = null)
        {
            var res = await _followQueryService.ListFollowingProfilesPage(userId, next);
            return Ok(res);
        }

        [HttpGet("list-followers-page/{userId}")]
        [ProducesResponseType(typeof(FollowsPageDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListFollowersProfiles(string userId, [FromQuery] string? next = null)
        {
            var res = await _followQueryService.ListFollowersProfilesPage(userId, next);
            return Ok(res);
        }
    }
}
