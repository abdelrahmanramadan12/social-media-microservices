using Domain.Entities;
using Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/public/feeds")]
    [ApiController]
    public class FeedController : ControllerBase
    {
        private readonly IFeedQueryService _feedQueryService;

        public FeedController(IFeedQueryService feedQueryService)
        {
            _feedQueryService = feedQueryService;
        }

        [HttpGet("timeline/{userId}")]
        [ProducesResponseType(typeof(List<Post>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTimeline(string userId)
        {
            var res = await _feedQueryService.GetUserTimeline(userId);
            return Ok(res);
        }
    }
}
