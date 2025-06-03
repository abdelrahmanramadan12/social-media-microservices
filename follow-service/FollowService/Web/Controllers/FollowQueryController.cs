using Microsoft.AspNetCore.Mvc;
using Application.Abstractions;
using Application.DTOs;
using Services.DTOs;
using Application.DTOs.Requests;

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

        [HttpPost("is-following")]
        [ProducesResponseType(typeof(CheckDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> IsFollowing([FromBody] IsFollowingRequest request)
        {
            var res = await _followQueryService.IsFollowing(request.UserId, request.OtherId);
            return Ok(new CheckDTO { Result = res });
        }

        [HttpPost("is-follower")]
        [ProducesResponseType(typeof(CheckDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> IsFollower([FromBody] IsFollowingRequest request)
        {
            var res = await _followQueryService.IsFollower(request.UserId, request.OtherId);
            return Ok(new CheckDTO { Result = res });
        }

        [HttpPost("list-following")]
        [ProducesResponseType(typeof(FollowsDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListFollowing([FromBody] ListFollowRequest request)
        {
            var res = await _followQueryService.ListFollowing(request.UserId);
            return Ok(res);
        }

        [HttpPost("list-followers")]
        [ProducesResponseType(typeof(FollowsDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListFollowers([FromBody] ListFollowRequest request)
        {
            var res = await _followQueryService.ListFollowers(request.UserId);
            return Ok(res);
        }

        [HttpPost("list-following-page")]
        [ProducesResponseType(typeof(FollowsPageDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListFollowingPage([FromBody] ListFollowPageRequest request)
        {
            var res = await _followQueryService.ListFollowingPage(request.UserId, request.Next);
            return Ok(res);
        }

        [HttpPost("list-followers-page")]
        [ProducesResponseType(typeof(FollowsPageDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListFollowersPage([FromBody] ListFollowPageRequest request)
        {
            var res = await _followQueryService.ListFollowersPage(request.UserId, request.Next);
            return Ok(res);
        }
    }
}
