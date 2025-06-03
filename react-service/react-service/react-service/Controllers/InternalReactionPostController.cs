using Microsoft.AspNetCore.Mvc;
using react_service.Application.DTO;
using react_service.Application.DTO.ReactionPost.Request;
using react_service.Application.Interfaces.Services;
using Web.Controllers;

namespace react_service.Controllers
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
            var reactedPosts = await _reactionService.FilterPostsReactedByUserAsync(request.PostIds, request.UserId);
            return HandleServiceResponse(reactedPosts);
        }

        [HttpPost("user")]
        public async Task<IActionResult> GetPostsReactedByUser([FromBody] GetPostsReactedByUserRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserId))
            {
            var errorResponse = new ResponseWrapper<object> {
                Errors = new List<string> { "User ID cannot be null or empty." },
                ErrorType = ErrorType.BadRequest
            };
            return HandleServiceResponse(errorResponse);
            }
            var result = await _reactionService.GetPostsReactedByUserAsync(request.UserId, request.Next);
            return HandleServiceResponse(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetUserIdsReactedToPost([FromBody] GetReactsOfPostRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.PostId))
            {
                var errorResponse = new ResponseWrapper<object> {
                    Errors = new List<string> { "Post ID cannot be null or empty." },
                    ErrorType = ErrorType.BadRequest
                };
                return HandleServiceResponse(errorResponse);
            }
            var result = await _reactionService.GetUserIdsReactedToPostAsync(request.PostId, request.Next, 10);
            return HandleServiceResponse(result);
        }       
     }
}