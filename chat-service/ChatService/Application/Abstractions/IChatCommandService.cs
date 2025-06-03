using Application.DTOs;

namespace Application.Abstractions
{
    public interface IChatCommandService
    {
        public Task SendMessageAsync(MessageDTO message);
        public Task EditMessageAsync(MessageDTO message);
        public Task DeleteMessageAsync(MessageDTO message);
        public Task CreateConversationAsync(NewConversationDTO conversation);
    }
}
