using Domain.Entities;

namespace Application.Abstractions
{
    public interface IMessageRepository
    {
        Task<Message> AddAsync(Message messageEntity);
        Task<bool> EditAsync(Message existingMessage);
        Task<Message> GetByIdAsync(string messageId);
        Task<List<Message>> GetMessagesPageAsync(string conversationId, string? next, int pageSize);
        Task MarkReadAsync(string userId, string conversationId);
        Task RemoveAsync(string messageId);
    }
}
