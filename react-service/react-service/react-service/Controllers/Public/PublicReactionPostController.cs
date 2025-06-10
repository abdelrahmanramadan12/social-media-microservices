using Microsoft.AspNetCore.Mvc;
using react_service.Application.DTO.Reaction.Request.Post;
using react_service.Application.Interfaces.Services;
using Web.Controllers;

namespace react_service.Controllers.Public
{
    [ApiController]
    [Route("api/public/reacts/post")]
    public class PublicReactionPostController : BaseController
    {
        private readonly IReactionPostService _reactionService;

        public PublicReactionPostController(IReactionPostService reactionService)
        {
            _reactionService = reactionService;
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteReaction([FromBody] DeletePostReactionRequest request, [FromHeader(Name = "userId")] string userId)
        {
            var res = await _reactionService.DeleteReactionAsync(request?.PostId, userId);
            if (res.Success)
            {
                return HandleResponse(res);
            }
            else
            {
                return HandleErrorResponse(res);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddReaction([FromBody] CreatePostReactionRequest request, [FromHeader(Name = "userId")] string userId)
        {
            var res = await _reactionService.AddReactionAsync(request, userId);
            if (res.Success)
            {
                return HandleResponse(res);
            }
            else
            {
                return HandleErrorResponse(res);
            }
        }
    }
}