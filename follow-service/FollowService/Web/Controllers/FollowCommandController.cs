using Microsoft.AspNetCore.Mvc;
using Application.Abstractions;
using Application.DTOs;
using Application.DTOs.Responses;

namespace Web.Controllers
{
    [Route("api/public/follow")]
    [ApiController]
    public class FollowCommandController : BaseController
    {
        private readonly IFollowCommandService _followCommandService;

        public FollowCommandController(IFollowCommandService followService)
        {
            _followCommandService = followService;
        }

        [HttpPost("follow")]
        [ProducesResponseType(typeof(ResponseWrapper<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Follow([FromHeader(Name = "userId")] string userId, [FromBody] FollowDTO req)
        {
            var result = await _followCommandService.Follow(userId, req.OtherId);
            var response = new ResponseWrapper<bool>
            {
                Data = result,
                Message = result ? "Successfully followed user" : "Failed to follow user"
            };
            
            if (!result)
            {
                response.ErrorType = ErrorType.BadRequest;
                response.Errors = new List<string> { "Unable to follow user" };
            }
            
            return HandleError(response);
        }

        [HttpDelete("unfollow")]
        [ProducesResponseType(typeof(ResponseWrapper<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Unfollow([FromHeader(Name = "userId")] string userId, [FromBody] FollowDTO req)
        {
            try
            {
                await _followCommandService.Unfollow(userId, req.OtherId);
                var response = new ResponseWrapper<bool>
                {
                    Data = true,
                    Message = "Successfully unfollowed user"
                };
                return HandleError(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseWrapper<bool>
                {
                    Data = false,
                    Message = "Failed to unfollow user",
                    ErrorType = ErrorType.InternalServerError,
                    Errors = new List<string> { ex.Message }
                };
                return HandleError(response);
            }
        }
    }
}
