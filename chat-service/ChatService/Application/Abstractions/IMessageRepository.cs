using Domain.Entities;

namespace Application.Abstractions
{
    public interface IMessageRepository
    {
        Task AddAsync(Message messageEntity);
        Task<Message> GetByIdAsync(string messageId);
        Task<List<Message>> GetMessagesPageAsync(string conversationId, string? next, int pageSize);
        Task RemoveAsync(Message message);
    }
}
