using Application.DTOs;
using Application.DTOs.Aggregation;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/public/[Controller]")]
    [ApiController]
    public class FollowController : BaseController
    {
        private readonly IFollowAggregationService _aggregationService;
        public FollowController(IFollowAggregationService aggregationService)
        {
            this._aggregationService = aggregationService;
        }

        [HttpPost("followers")]
        public async Task<ActionResult<PaginationResponseWrapper<ProfileAggregation>>> GetFollowers([FromBody] FollowListRequest request, [FromHeader(Name = "userId")] string userId)
        {
            request.UserId = userId;

            var response = await _aggregationService.GetFollowers(request);
            if (!response.Success)
            {
                return HandlePaginationErrorResponse(response);
            }
            return HandlePaginationResponse(response);
        }

        [HttpPost("following")]
        public async Task<ActionResult<PaginationResponseWrapper<ProfileAggregation>>> GetFollowing([FromBody] FollowListRequest request, [FromHeader(Name = "userId")] string userId)
        {
            request.UserId = userId;

            var response = await _aggregationService.GetFollowers(request);
            if (!response.Success)
            {
                return HandlePaginationErrorResponse(response);
            }
            return HandlePaginationResponse(response);
        }

    }
}
