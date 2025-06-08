using Application.Abstractions;
using Application.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatPublicController : ControllerBase
    {
        private readonly IChatCommandService _chatCommandService;
        public ChatPublicController(IChatCommandService chatCommandService)
        {
            _chatCommandService = chatCommandService;
        }

        [HttpPost("message")]
        public async Task<IActionResult> SendMessage([FromHeader] string userId ,[FromBody] NewMessageDTO message)
        {
            if (message ==null)
            {
                return BadRequest("Message cannot be empty.");
            }
            try
            {
                message.SenderId = userId;
                var response = await _chatCommandService.SendMessageAsync( message);
                return Ok(response);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest( $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("message")]
        public async Task<IActionResult> EditMessage([FromHeader] string userId ,[FromBody] MessageDTO message)
        {
            if (message == null || string.IsNullOrEmpty(message.Id))
            {
                return BadRequest("Message cannot be null or empty.");
            }
            try
            {
                message.SenderId = userId;
                var response = await _chatCommandService.EditMessageAsync(message);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("message/{Id}")]
        public async Task<IActionResult> DeleteMessage(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return BadRequest("Message ID cannot be null or empty.");
            }
            try
            {
                await _chatCommandService.DeleteMessageAsync(Id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("mark-read")]
        public async Task<IActionResult> MarkRead([FromBody] MarkReadRequestDTO request, [FromHeader] string userId)
        {
            await _chatCommandService.MarkReadAsync(userId, request.ConversationId);
            return NoContent();
        }

        /*------------------------------------------------------------------*/
        /*---------------------Endpoints for conversations------------------*/
        /*------------------------------------------------------------------*/
        [HttpPost("conversation")]
        public async Task<IActionResult> CreateConversation([FromHeader] string userId, [FromBody] NewConversationDTO conversation)
        {
            if (conversation == null)
            {
                return BadRequest("Conversation cannot be null.");
            }
            try
            {
                conversation.UserId = userId;
                var response = await _chatCommandService.CreateConversationAsync(conversation);
                return Ok(response);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("conversation/{Id}")]
        public async Task<IActionResult> DeleteConversation([FromHeader] string userId , string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return BadRequest("Conversation ID cannot be null or empty.");
            }
            try
            {
                await _chatCommandService.DeleteConversationAsync(userId,Id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

    }
}
