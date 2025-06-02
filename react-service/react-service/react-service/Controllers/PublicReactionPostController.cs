using Microsoft.AspNetCore.Mvc;
using react_service.Application.DTO;
using react_service.Application.DTO.ReactionPost.Request;
using react_service.Application.Interfaces.Services;
using Web.Controllers;

namespace react_service.Controllers
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
        public async Task<IActionResult> DeleteReaction([FromBody] DeleteReactionRequest request, [FromHeader(Name = "userId")] string userId)
        {
            var res = await _reactionService.DeleteReactionAsync(request?.PostId, userId);
            return HandleServiceResponse<object>(res);
        }

        [HttpPost]
        public async Task<IActionResult> AddReaction([FromBody] CreateReactionRequest request, [FromHeader(Name = "userId")] string userId)
        {
            var res = await _reactionService.AddReactionAsync(request, userId);
            return HandleServiceResponse<object>(res);
        }
    }
}