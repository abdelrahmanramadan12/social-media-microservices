using Microsoft.AspNetCore.Mvc;
using Application.Abstractions;
using Application.DTOs.Responses;
using Services.DTOs.Requests;
using System.Net;

namespace Web.Controllers
{
    [Route("api/internal/follow")]
    [ApiController]
    public class FollowQueryController : BaseController
    {
        private readonly IFollowQueryService _followQueryService;

        public FollowQueryController(IFollowQueryService followService)
        {
            _followQueryService = followService;
        }

        [HttpPost("is-following")]
        public async Task<IActionResult> IsFollowing([FromBody] IsFollowingRequest request)
        {
            var result = await _followQueryService.IsFollowing(request.UserId, request.OtherId);
            var response = new ResponseWrapper<bool>
            {
                Data = result,
                Message = "Successfully checked follow status"
            };

            if (!response.Success)
            {
                return HandleError(response);
            }
            return Ok(new { data = response.Data, message = response.Message });
        }

        [HttpPost("is-follower")]
        public async Task<IActionResult> IsFollower([FromBody] IsFollowingRequest request)
        {
            var result = await _followQueryService.IsFollower(request.UserId, request.OtherId);
            var response = new ResponseWrapper<bool>
            {
                Data = result,
                Message = "Successfully checked follower status"
            };

            if (!response.Success)
            {
                return HandleError(response);
            }
            return Ok(new { data = response.Data, message = response.Message });
        }

        [HttpPost("~/api/public/follow/is-following")]
        public async Task<IActionResult> IsFollowingPublic([FromHeader(Name = "userId")] string userId, [FromBody] IsFollowingRequestPublic request)
        {
            var result = await _followQueryService.IsFollowing(userId, request.OtherId);
            var response = new ResponseWrapper<bool>
            {
                Data = result,
                Message = "Successfully checked follow status"
            };

            if (!response.Success)
            {
                return HandleError(response);
            }
            return Ok(new { data = response.Data, message = response.Message });
        }

        [HttpPost("~/api/public/follow/is-follower")]
        public async Task<IActionResult> IsFollowerPublic([FromHeader(Name = "userId")] string userId, [FromBody] IsFollowingRequestPublic request)
        {
            var result = await _followQueryService.IsFollower(userId, request.OtherId);
            var response = new ResponseWrapper<bool>
            {
                Data = result,
                Message = "Successfully checked follower status"
            };

            if (!response.Success)
            {
                return HandleError(response);
            }
            return Ok(new { data = response.Data, message = response.Message });
        }


        [HttpPost("list-following")]
        public async Task<IActionResult> ListFollowing([FromBody] ListFollowRequest request)
        {
            var result = await _followQueryService.ListFollowing(request.UserId);
            var response = new ResponseWrapper<ICollection<string>>
            {
                Data = result.Follows?.ToList() ?? new List<string>(),
                Message = "Successfully retrieved following list"
            };

            if (!response.Success)
            {
                return HandleError(response);
            }
            return Ok(new { data = response.Data, message = response.Message });
        }

        [HttpPost("list-followers")]
        public async Task<IActionResult> ListFollowers([FromBody] ListFollowRequest request)
        {
            var result = await _followQueryService.ListFollowers(request.UserId);
            var response = new ResponseWrapper<ICollection<string>>
            {
                Data = result.Follows?.ToList() ?? new List<string>(),
                Message = "Successfully retrieved followers list"
            };

            if (!response.Success)
            {
                return HandleError(response);
            }
            return Ok(new { data = response.Data, message = response.Message });
        }

        [HttpPost("list-following-page")]
        public async Task<IActionResult> ListFollowingPage([FromBody] ListFollowPageRequest request)
        {
            var result = await _followQueryService.ListFollowingPage(request.UserId, request.Next);
            var response = new ResponseWrapper<ICollection<string>>
            {
                Data = result.Follows?.ToList() ?? new List<string>(),
                Message = "Successfully retrieved paginated following list"
            };

            if (!response.Success)
            {
                return HandleError(response);
            }

            return Ok(new
            {
                data = response.Data ?? new List<string>(),
                next = result.Next ?? string.Empty,
                hasMore = !string.IsNullOrEmpty(result.Next),
                message = response.Message
            });
        }

        [HttpPost("list-followers-page")]
        public async Task<IActionResult> ListFollowersPage([FromBody] ListFollowPageRequest request)
        {
            var result = await _followQueryService.ListFollowersPage(request.UserId, request.Next);
            var response = new ResponseWrapper<ICollection<string>>
            {
                Data = result.Follows?.ToList() ?? new List<string>(),
                Message = "Successfully retrieved paginated followers list"
            };

            if (!response.Success)
            {
                return HandleError(response);
            }

            return Ok(new
            {
                data = response.Data ?? new List<string>(),
                next = result.Next ?? string.Empty,
                hasMore = !string.IsNullOrEmpty(result.Next),
                message = response.Message
            });
        }

        [HttpPost("filter-followers")]
        public async Task<IActionResult> FilterFollowers([FromBody] FilterFollowStatusRequest request)
        {
            var result = await _followQueryService.FilterFollowers(request.UserId, request.OtherIds);
            var response = new ResponseWrapper<ICollection<string>>
            {
                Data = result ?? new List<string>(),
                Message = "Successfully filtered followers"
            };
            if (!response.Success)
            {
                return HandleError(response);
            }
            return Ok(new { data = response.Data, message = response.Message });
        }

        [HttpPost("filter-following")]
        public async Task<IActionResult> FilterFollowings([FromBody] FilterFollowStatusRequest request)
        {
            var result = await _followQueryService.FilterFollowings(request.UserId, request.OtherIds);
            var response = new ResponseWrapper<ICollection<string>>
            {
                Data = result ?? new List<string>(),
                Message = "Successfully filtered followings"
            };
            if (!response.Success)
            {
                return HandleError(response);
            }
            return Ok(new { data = response.Data, message = response.Message });
        }
    }
}
