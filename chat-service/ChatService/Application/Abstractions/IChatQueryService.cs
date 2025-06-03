using Application.DTOs;

namespace Application.Abstractions
{
    public interface IChatQueryService
    {
        public Task<ConversationMessagesDTO> GetConversationMessagesAsync(string conversationId, string? next);
        public Task<UserConversationsDTO> GetUserConversationsAsync(string userId, string? next);
    }
}
