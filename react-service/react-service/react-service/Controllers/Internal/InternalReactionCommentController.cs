using Microsoft.AspNetCore.Mvc;
using react_service.Application.DTO;
using react_service.Application.DTO.Reaction.Request.Comment;
using react_service.Application.DTO.ReactionPost.Request.Comment;
using react_service.Application.DTO.ReactionPost.Request.Post;
using react_service.Application.Interfaces.Services;
using Web.Controllers;

namespace react_service.Controllers.Internal
{
    [ApiController]
    [Route("api/internal/reacts/comment")]
    public class InternalReactionCommentController : BaseController
    {
        private readonly IReactionCommentService _reactionService;
        private readonly IConfiguration _configuration;
        private readonly int _defaultPageSize = 10;

        public InternalReactionCommentController(IReactionCommentService reactionService, IConfiguration configuration)
        {
            _reactionService = reactionService;
            _configuration = configuration;
            if (_configuration != null && _configuration["Pagination:DefaultPageSize"] != null)
            {
                _defaultPageSize = int.Parse(_configuration["Pagination:DefaultPageSize"]);
            }
        }

        [HttpPost("filter")]
        public async Task<IActionResult> FilterCommentsReactedByUser([FromBody] FilterCommentsReactedByUserRequest request)
        {
            var response = await _reactionService.FilterCommentsReactedByUserAsync(request.CommentIds, request.UserId);
            if (!response.Success)
            {
                return HandleErrorResponse(response);
            }
            return HandleResponse(response);
        }

        [HttpPost("user")]
        public async Task<IActionResult> GetCommentsReactedByUser([FromBody] GetReactionsByUserRequest request)
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
            var response = await _reactionService.GetCommentsReactedByUserAsync(request.UserId, request.Next);
            if (!response.Success)
            {
                return HandlePaginationErrorResponse(response);
            }
            return HandlePaginationResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> GetUserIdsReactedToComment([FromBody] GetReactsOfCommentRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.CommentId))
            {
                var errorResponse = new ResponseWrapper<List<string>>
                {
                    Errors = new List<string> { "Comment ID cannot be null or empty." },
                    ErrorType = ErrorType.BadRequest
                };
                return HandleErrorResponse(errorResponse);
            }
            var response = await _reactionService.GetUserIdsReactedToCommentAsync(request.CommentId, request.Next, _defaultPageSize);
            if (!response.Success)
            {
                return HandlePaginationErrorResponse(response);
            }
            return HandlePaginationResponse(response);
        }

        [HttpPost("is-liked")]
        public async Task<IActionResult> IsCommentLikedByUser([FromBody] IsCommentLikedByUserRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.CommentId) || string.IsNullOrEmpty(request.UserId))
            {
                var errorResponse = new ResponseWrapper<object>
                {
                    Errors = new List<string> { "Comment ID and User ID cannot be null or empty." },
                    ErrorType = ErrorType.BadRequest
                };
                return HandleErrorResponse(errorResponse);
            }
            var response = await _reactionService.IsCommentReactedByUserAsync(request.CommentId, request.UserId);
            if (!response.Success)
            {
                return HandleErrorResponse(response);
            }
            return HandleResponse(response);
        }
    }
}