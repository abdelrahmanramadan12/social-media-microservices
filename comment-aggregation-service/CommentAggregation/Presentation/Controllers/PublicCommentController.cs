using Application.DTOs.Comment;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/public/comment")]
    [ApiController]
    public class PublicCommentController : BaseController
    {
        private readonly ICommentAggregationService _commentAggregationService;
        public PublicCommentController(ICommentAggregationService commentAggregationService)
        {
            this._commentAggregationService = commentAggregationService ?? throw new ArgumentNullException(nameof(commentAggregationService));
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetCommentList([FromHeader(Name ="userId")] string userId ,[FromBody] GetPagedCommentRequest request)
        {
            request.UserId = userId;
            var response = await _commentAggregationService.GetCommentList(request);
            if (!response.Success)
            {
                return HandlePaginationErrorResponse(response);
            }
            return HandlePaginationResponse(response);
        }
    }
}
