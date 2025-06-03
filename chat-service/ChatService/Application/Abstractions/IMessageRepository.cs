using Domain.Entities;

namespace Application.Abstractions
{
    public interface IMessageRepository
    {
        Task<List<Message>> GetMessagesPageAsync(string conversationId, string? next, int pageSize);
    }
}
