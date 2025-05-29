using Microsoft.AspNetCore.Mvc;
using Application.Abstractions;
using Application.DTOs;

namespace Web.Controllers
{
    [Route("api/public/follows")]
    [ApiController]
    public class FollowCommandController : ControllerBase
    {
        private readonly IFollowCommandService _followCommandService;

        public FollowCommandController(IFollowCommandService followService)
        {
            _followCommandService = followService;
        }

        [HttpPost("follow")]
        [ProducesResponseType(typeof(void),StatusCodes.Status201Created)]
        public async Task<IActionResult> Follow([FromHeader(Name = "userId")] string userId, [FromBody] FollowDTO req)
        {
            var res = await _followCommandService.Follow(userId, req.OtherId);

            if (res)
            {
                return Created();
            }

            return BadRequest();
        }

        [HttpDelete("unfollow")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Unfollow([FromHeader(Name = "userId")] string userId, [FromBody] FollowDTO req)
        {
            await _followCommandService.Unfollow(userId, req.OtherId);
            return NoContent();
        }
    }
}
