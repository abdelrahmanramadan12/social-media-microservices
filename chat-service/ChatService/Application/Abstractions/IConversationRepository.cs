
using Domain.Entities;

namespace Application.Abstractions
{
    public interface IConversationRepository
    {
        Task<Conversation> AddAsync(Conversation conversationEntity);
        Task<List<Conversation>> GetConversationsAsync(string userId, string? next, int pageSize);
    }
}
