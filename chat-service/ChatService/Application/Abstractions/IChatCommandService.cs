using Application.DTOs;

namespace Application.Abstractions
{
    public interface IChatCommandService
    {
        public Task SendMessageAsync(NewMessageDTO message);
        public Task EditMessageAsync(MessageDTO message);
        public Task DeleteMessageAsync(string messageId);
        public Task CreateConversationAsync(NewConversationDTO conversation);
    }
}
