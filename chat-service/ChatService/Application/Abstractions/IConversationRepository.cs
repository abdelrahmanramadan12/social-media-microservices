
using Domain.Entities;

namespace Application.Abstractions
{
    public interface IConversationRepository
    {
        Task<Conversation> AddAsync(Conversation conversationEntity);
        Task <Conversation>EditAsync(Conversation existingConversation);
        Task<Conversation> GetConversationByIdAsync(string id);
        Task<List<Conversation>> GetConversationsAsync(string userId, string? next, int pageSize);
        Task RemoveAsync(string conversationId);
    }
}
