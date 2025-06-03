
using Domain.Entities;

namespace Application.Abstractions
{
    public interface IConversationRepository
    {
        Task AddAsync(Conversation conversationEntity);
        Task<List<Conversation>> GetConversationsPageAsync(string userId, string? next, int pageSize);
    }
}
