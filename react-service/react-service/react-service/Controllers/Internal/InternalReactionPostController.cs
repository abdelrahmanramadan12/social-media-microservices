using Microsoft.AspNetCore.Mvc;
using react_service.Application.DTO;
using react_service.Application.DTO.Reaction.Request.Post;
using react_service.Application.DTO.ReactionPost.Request.Post;
using react_service.Application.Interfaces.Services;
using Web.Controllers;

namespace react_service.Controllers.Internal
{
    [ApiController]
    [Route("api/internal/reacts/post")]
    public class InternalReactionPostController : BaseController
    {
        private readonly IReactionPostService _reactionService;

        public InternalReactionPostController(IReactionPostService reactionService)
        {
            _reactionService = reactionService;
        }

        [HttpPost("filter")]
        public async Task<IActionResult> FilterPostsReactedByUser([FromBody] FilterPostsReactedByUserRequest request)
        {
            var response = await _reactionService.FilterPostsReactedByUserAsync(request.PostIds, request.UserId);
            if (!response.Success)
            {
                return HandleErrorResponse(response);
            }
            return HandleResponse(response);
        }

        [HttpPost("user")]
        public async Task<IActionResult> GetPostsReactedByUser([FromBody] GetReactionsByUserRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserId))
            {
                var errorResponse = new ResponseWrapper<object>
                {
                    Errors = new List<string> { "User ID cannot be null or empty." },
                    ErrorType = ErrorType.BadRequest
                };
                return HandleErrorResponse(errorResponse);
            }
            var response = await _reactionService.GetPostsReactedByUserAsync(request.UserId, request.Next);
            if (!response.Success)
            {
                return HandlePaginationErrorResponse(response);
            }
            return HandlePaginationResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> GetUserIdsReactedToPost([FromBody] GetReactsOfPostRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.PostId))
            {
                var errorResponse = new ResponseWrapper<List<string>>
                {
                    Errors = new List<string> { "Post ID cannot be null or empty." },
                    ErrorType = ErrorType.BadRequest
                };
                return HandleErrorResponse(errorResponse);
            }
            var response = await _reactionService.GetUserIdsReactedToPostAsync(request.PostId, request.Next, 10);
            if (!response.Success)
            {
                return HandlePaginationErrorResponse(response);
            }
            return HandlePaginationResponse(response);
        }

        [HttpPost("is-liked")]
        public async Task<IActionResult> IsPostLikedByUser([FromBody] IsPostLikedByUserRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.PostId) || string.IsNullOrEmpty(request.UserId))
            {
                var errorResponse = new ResponseWrapper<object>
                {
                    Errors = new List<string> { "Post ID and User ID cannot be null or empty." },
                    ErrorType = ErrorType.BadRequest
                };
                return HandleErrorResponse(errorResponse);
            }
            var response = await _reactionService.IsPostReactedByUserAsync(request.PostId, request.UserId);
            if (!response.Success)
            {
                return HandleErrorResponse(response);
            }
            return HandleResponse(response);
        }
    }
}