using Application.Abstractions;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatInternalController : ControllerBase
    {
        private readonly IChatQueryService _chatQueryService;
        public ChatInternalController(IChatQueryService chatQueryService)
        {
            _chatQueryService = chatQueryService;
        }

        [HttpGet("messages")]
        public async Task<IActionResult> GetMessagesPage([FromBody] MessagesPageRequestDTO  messagesPageRequestDTO)
        {
            if (string.IsNullOrEmpty(messagesPageRequestDTO.ConversationId))
            {
                return BadRequest("Conversation ID cannot be null or empty.");
            }
            try
            {
                var messages = await _chatQueryService.GetConversationMessagesAsync(
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

        [HttpGet("conversations")]
        public async Task<IActionResult> GetUserConversations([FromBody] ConversationsPageRequestDTO conversationsPageRequestDTO)
        {
            if (string.IsNullOrEmpty(conversationsPageRequestDTO.UserId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }
            try
            {
                var conversations = await _chatQueryService.GetUserConversationsAsync(
                    conversationsPageRequestDTO.UserId,conversationsPageRequestDTO.Next,
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
