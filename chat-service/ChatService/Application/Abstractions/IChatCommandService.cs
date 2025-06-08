using Application.DTOs;

namespace Application.Abstractions
{
    public interface IChatCommandService
    {
        public Task<MessageDTO> SendMessageAsync(NewMessageDTO message);
        public Task<MessageDTO> EditMessageAsync(MessageDTO message);
        public Task DeleteMessageAsync(string userId , string messageId);
        public Task MarkReadAsync(string userId, string conversationId);
        public Task<ConversationDTO> CreateConversationAsync(NewConversationDTO conversation);
        Task DeleteConversationAsync(string userId, string conversationId);
        Task<ConversationDTO> EditConversationAsync(EditConversationDTO conversation);
    }
}
