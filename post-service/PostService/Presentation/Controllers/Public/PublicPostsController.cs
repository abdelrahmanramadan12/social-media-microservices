using Application.DTOs;
using Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Public
{
    [Route("api/public/post")]
    [ApiController]
    public class PublicPostController : BaseController
    {
        private readonly IPostService _postService;
        private readonly IEncryptionService _encryptionService;
        public PublicPostController(IPostService postSerive, IEncryptionService encryptionService)
        {
            this._postService = postSerive;
            this._encryptionService = encryptionService;
        }

        [HttpPost]
        public async Task<IActionResult> AddPost([FromForm] PostInputDTO postDto, [FromHeader(Name = "userId")] string userId)
        {
            var res = await _postService.AddPostAsync(userId, postDto);
            if (!res.Success)
            {
                return HandleErrorResponse<PostResponseDTO>(res);
            }
            return HandleResponse<PostResponseDTO>(res);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePost([FromForm] PostInputDTO postDto, [FromHeader(Name = "userId")] string userId)
        {
            var res = await _postService.UpdatePostAsync(userId, postDto);
            if (!res.Success)
            {
                return HandleErrorResponse<PostResponseDTO>(res);
            }
            return HandleResponse<PostResponseDTO>(res);
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePost([FromBody] DeletePostRequest request, [FromHeader(Name = "userId")] string userId)
        {
            var res = await _postService.DeletePostAsync(userId, request.PostId);
            if (!res.Success)
            {
                return HandleErrorResponse<string>(res);
            }
            return HandleResponse<string>(res);
        }
    }
}
