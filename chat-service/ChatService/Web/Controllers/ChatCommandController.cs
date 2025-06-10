using Application.Abstractions;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatCommandController : ControllerBase
    {
        private readonly IChatCommandService _chatCommandService;
        public ChatCommandController(IChatCommandService chatCommandService)
        {
            _chatCommandService = chatCommandService;
        }

        [HttpPost("message")]
        [ProducesResponseType(typeof(MessageDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> SendMessage([FromHeader] string userId, [FromBody] NewMessageDTO message)
        {
            if (message == null)
            {
                return BadRequest("Message cannot be empty.");
            }
            try
            {
                message.SenderId = userId;
                var response = await _chatCommandService.SendMessageAsync(message);
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
                return StatusCode(StatusCodes.Status403Forbidden, $"Forbidden: {ex.Message}");
                //return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("message")]
        [ProducesResponseType(typeof(MessageDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> EditMessage([FromHeader] string userId, [FromBody] MessageDTO message)
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
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, $"Forbidden: {ex.Message}");
                //return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("message/{Id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteMessage([FromHeader] string userId, string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return BadRequest("Message ID cannot be null or empty.");
            }
            try
            {
                await _chatCommandService.DeleteMessageAsync(userId, Id);
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
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MarkRead([FromBody] MarkReadRequestDTO request, [FromHeader] string userId)
        {
            await _chatCommandService.MarkReadAsync(userId, request.ConversationId);
            return NoContent();
        }

        /*------------------------------------------------------------------*/
        /*---------------------Endpoints for conversations------------------*/
        /*------------------------------------------------------------------*/
        [HttpPost("conversation")]
        [ProducesResponseType(typeof(ConversationDTO), StatusCodes.Status200OK)]
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

        [HttpPatch("conversation")]
        [ProducesResponseType(typeof(ConversationDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> EditConversation([FromHeader] string userId, [FromBody] EditConversationDTO conversation)
        {
            if (conversation == null || string.IsNullOrEmpty(conversation.Id))
            {
                return BadRequest("Conversation cannot be null or empty.");
            }
            try
            {
                conversation.UserId = userId;
                var response = await _chatCommandService.EditConversationAsync(conversation);
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
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, $"Forbidden: {ex.Message}");
                //return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("conversation/{Id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteConversation([FromHeader] string userId, string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return BadRequest("Conversation ID cannot be null or empty.");
            }
            try
            {
                await _chatCommandService.DeleteConversationAsync(userId, Id);
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
