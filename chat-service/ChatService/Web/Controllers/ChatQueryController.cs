using Application.Abstractions;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatQueryController : ControllerBase
    {
        private readonly IChatQueryService _chatQueryService;
        public ChatQueryController(IChatQueryService chatQueryService)
        {
            _chatQueryService = chatQueryService;
        }

        [HttpPost("messages")]
        [ProducesResponseType(typeof(ConversationMessagesDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMessagesPage([FromHeader] string userId, [FromBody] MessagesPageRequestDTO  messagesPageRequestDTO)
        {
            if (string.IsNullOrEmpty(messagesPageRequestDTO.ConversationId))
            {
                return BadRequest("Conversation ID cannot be null or empty.");
            }
            try
            {
                var messages = await _chatQueryService.GetConversationMessagesAsync(
                    userId,
                    messagesPageRequestDTO.ConversationId, messagesPageRequestDTO.Next,
                    messagesPageRequestDTO.PageSize);
                return Ok(messages);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("conversations")]
        [ProducesResponseType(typeof(UserConversationsDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserConversations([FromHeader] string userId, [FromBody] ConversationsPageRequestDTO conversationsPageRequestDTO)
        {
            try
            {
                var conversations = await _chatQueryService.GetUserConversationsAsync(
                    userId, conversationsPageRequestDTO.Next,
                    conversationsPageRequestDTO.PageSize);
                return Ok(conversations);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }

    }
}
