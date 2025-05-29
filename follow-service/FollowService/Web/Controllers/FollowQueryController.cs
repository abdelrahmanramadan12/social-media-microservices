using Microsoft.AspNetCore.Mvc;
using Application.Abstractions;
using Application.DTOs;

namespace Web.Controllers
{
    [Route("api/internal/follows")]
    [ApiController]
    public class FollowQueryController : ControllerBase
    {
        private readonly IFollowQueryService _followQueryService;

        public FollowQueryController(IFollowQueryService followService)
        {
            _followQueryService = followService;
        }

        [HttpGet("is-following/{userId}/{otherId}")]
        [ProducesResponseType(typeof(CheckDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> IsFollowing(string userId, string otherId)
        {
            var res = await _followQueryService.IsFollowing(userId, otherId);
            return Ok(new CheckDTO { Result = res });
        }

        [HttpGet("is-follower/{userId}/{otherId}")]
        [ProducesResponseType(typeof(CheckDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> IsFollower(string userId, string otherId)
        {
            var res = await _followQueryService.IsFollower(userId, otherId);
            return Ok(new CheckDTO { Result = res });
        }

        [HttpGet("list-following/{userId}")]
        [ProducesResponseType(typeof(FollowsDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListFollowing(string userId)
        {
            var res = await _followQueryService.ListFollowing(userId);
            return Ok(res);
        }

        [HttpGet("list-followers/{userId}")]
        [ProducesResponseType(typeof(FollowsDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListFollowers(string userId)
        {
            var res = await _followQueryService.ListFollowers(userId);
            return Ok(res);
        }

        [HttpGet("list-following-page/{userId}")]
        [ProducesResponseType(typeof(FollowsPageDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListFollowingPage(string userId,[FromQuery] string? next = null)
        {
            var res = await _followQueryService.ListFollowingPage(userId, next);
            return Ok(res);
        }

        [HttpGet("list-followers-page/{userId}")]
        [ProducesResponseType(typeof(FollowsPageDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListFollowersPage(string userId,[FromQuery] string? next = null)
        {
            var res = await _followQueryService.ListFollowersPage(userId, next);
            return Ok(res);
        }
    }
}
