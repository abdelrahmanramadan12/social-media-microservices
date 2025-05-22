using System.Xml.Linq;
using Domain.DTOs;
using Domain.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Service.Interfaces.CommentServices;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        //private readonly ICommentRepository _commentRepository;
        private readonly ICommentService _commentService;
        public CommentController(ICommentRepository commentRepository, ICommentService commentService)
        {
            //_commentRepository = commentRepository;
            _commentService = commentService;
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> Get(
            [FromRoute] string postId,
            [FromQuery] string? next = null)
        {
            var comments = await _commentService.ListCommentsAsync(postId, next);

            return Ok(comments);
        }


        [HttpPost]
        public async Task<IActionResult> Create(
        [FromForm] CreateCommentRequestDto createCommentRequestDto,
        [FromHeader(Name = "userId")] string userId)
        {
            createCommentRequestDto.UserId=userId;

            try
            {
                var result = await _commentService.CreateCommentAsync(createCommentRequestDto);
                if (result == null)
                    return BadRequest("Failed to create comment.");
                return Created(string.Empty, result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> EditComment(
        [FromForm] EditCommentRequestDto editCommentRequestDto,
        [FromHeader(Name = "userId")] string userId)
            {
                
                try
                {
                editCommentRequestDto.UserId = userId;        

                    var result = await _commentService.UpdateCommentAsync(editCommentRequestDto);
                    if (result == null)
                        return NotFound();

                    return Ok(result);
                }
                catch (UnauthorizedAccessException ex)
                {
                    return BadRequest(ex.Message);
                }
            }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, [FromHeader (Name = "userId")] string userId)
        {
            try
            {
                var deleted = await _commentService.DeleteCommentAsync(id, userId);
                if (!deleted)
                    return NotFound();

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
