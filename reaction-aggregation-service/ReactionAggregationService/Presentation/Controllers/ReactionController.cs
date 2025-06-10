using Application.DTOs.Reactions;
using Application.Services.Implementations;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/public/reacts")]
    [ApiController]
    public class ReactionController : BaseController
    {
        private readonly IReactionsAggregationService _reactionsAggregationService;
        public ReactionController(IReactionsAggregationService reactionsAggregationService)
        {
            _reactionsAggregationService = reactionsAggregationService;
        }
        [HttpPost("post")]
        public async Task<ActionResult> GetReactsOfPost([FromBody] GetReactsOfPostRequest request, [FromHeader(Name = "userId")] string userId)
        {
            request.UserId = userId;
            var response = await _reactionsAggregationService.GetReactionsOfPostAsync(request);
            if (!response.Success)
            {
                return HandlePaginationErrorResponse(response);
            }
            return HandlePaginationResponse(response);
        }
    }
}
