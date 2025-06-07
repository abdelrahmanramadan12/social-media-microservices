using Application.DTOs.Aggregation;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/public/[controller]")]
    [ApiController]
    public class PostsController : BaseController
    {
        private readonly IPostAggregationService _postAggregationService;
        public PostsController(IPostAggregationService postAggregationService)
        {
            this._postAggregationService = postAggregationService;
        }

        [HttpGet("user/{OtherId}")]
        public async Task<IActionResult> GetProfilePosts(string OtherId, [FromHeader(Name = "UserId")] string userId, [FromQuery] string? next)
        {
            var response = await _postAggregationService.GetProfilePosts(new ProfilePostsRequest
            {
                UserId = userId,
                OtherId = OtherId,
                Next = next ?? string.Empty
            });

            if (!response.Success)
            {
                return HandlePaginationErrorResponse(response);
            }

            return HandlePaginationResponse(response);
        }

        [HttpGet("reacted   ")]
        public async Task<IActionResult> GetReactedPosts([FromHeader(Name = "UserId")] string userId, [FromQuery] string? next)
        {
            var response = await _postAggregationService.GetReactedPosts(new ReactedPostsRequest
            {
                UserId = userId,
                Next = next ?? string.Empty
            });

            if (!response.Success)
            {
                return HandlePaginationErrorResponse(response);
            }

            return HandlePaginationResponse(response);
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> GetSinglePost(string postId, [FromHeader(Name = "UserId")] string userId)
        {
            var response = await _postAggregationService.GetSinglePost(new GetSinglePostRequest
            {
                PostId = postId,
                UserId = userId
            });

            if (!response.Success)
            {
                return HandleErrorResponse(response);
            }

            return HandleResponse(response);
        }
    }
}
