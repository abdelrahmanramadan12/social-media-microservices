using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Web.DTOs;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController : ControllerBase
    {
        private readonly IFollowService _followService;

        public FollowController(IFollowService followService)
        {
            _followService = followService;
        }

        [HttpPost("follow")]
        public IActionResult Follow(FollowDTO req)
        {
            _followService.Follow(req.UserId, req.OtherId);
            return NoContent();
        }

        [HttpPost("unfollow")]
        public IActionResult Unfollow(FollowDTO req)
        {
            _followService.Unfollow(req.UserId, req.OtherId);
            return NoContent();
        }

        [HttpGet("is-following/{userId}/{otherId}")]
        public IActionResult IsFollowing(string userId, string otherId)
        {
            var res = _followService.IsFollowing(userId, otherId);
            return Ok(new CheckDTO { Result = res});
        }

        [HttpGet("is-follower/{userId}/{otherId}")]
        public IActionResult IsFollower(string userId, string otherId)
        {
            var res = _followService.IsFollower(userId, otherId);
            return Ok(new CheckDTO { Result = res });
        }

        [HttpGet("list-following/{userId}")]
        public IActionResult ListFollowing(string userId)
        {
            var res = _followService.ListFollowing(userId);
            return Ok(new FollowingDTO { Following = res });
        }

        [HttpGet("list-followers/{userId}")]
        public IActionResult ListFollowers(string userId)
        {
            var res = _followService.ListFollowers(userId);
            return Ok(new FollowersDTO { Followers = res });
        }
    }
}
