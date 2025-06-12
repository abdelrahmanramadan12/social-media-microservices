using Application.DTOs;
using Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Internal
{
    [Route("api/internal/posts")]
    [ApiController]
    public class InternalPostsController : BaseController
    {
        private readonly IPostService _postService;
        private readonly IEncryptionService _encryptionService;
        private readonly int _defaultPageSize = 15;
        private readonly IConfiguration _configuration;
        public InternalPostsController(IPostService postSerive, IEncryptionService encryptionService, IConfiguration configuration)
        {
            _postService = postSerive;
            _encryptionService = encryptionService;
            _configuration = configuration;
            if (_configuration != null && _configuration["Pagination:DefaultPageSize"] != null)
            {
                _defaultPageSize = int.Parse(_configuration["Pagination:DefaultPageSize"]);
            }
        }
        // Endpoints
        [HttpGet("{postId}")]
        public async Task<IActionResult> GetPost(string postId)
        {
            var res = await _postService.GetPostByIdAsync(postId);
            return HandleResponse(res);
        }

        [HttpPost("user")]
        public async Task<IActionResult> GetProfilePostList([FromBody] GetProfilePostListRequest request)
        {

            var res = await _postService.GetProfilePostListAsync(request.UserId, request.ProfileUserId, _defaultPageSize, request.Next);

            if (!res.Success)
            {
                return HandlePaginationErrorResponse<List<PostResponseDTO>>(res);
            }
            return HandlePaginationResponse<List<PostResponseDTO>>(res);
       }

        [HttpPost("list")]
        public async Task<IActionResult> GetPostList([FromBody] GetPostListRequest request)
        {
            var res = await _postService.GetPostListAsync(request.UserId, request.PostIds);
            if (!res.Success)
            {
                return HandleErrorResponse<List<PostResponseDTO>>(res);
            }
            return HandleResponse<List<PostResponseDTO>>(res);

        }
    }
}
