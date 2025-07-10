using Application.Abstractions;
using Domain.Entities;
using MongoDB.Driver;

namespace Infrastructure.Repositories
{
    public class ConversationsRepository : IConversationRepository
    {
        private readonly IMongoCollection<Conversation> _conversations;

        public ConversationsRepository(IMongoDatabase db)
        {
            _conversations = db.GetCollection<Conversation>("conversations");
        }

        public async Task<Conversation> AddAsync(Conversation conversationEntity)
        {
            await _conversations.InsertOneAsync(conversationEntity);
            return conversationEntity;
        }

        public async Task<Conversation> EditAsync(Conversation existingConversation)
        {
            var filter = Builders<Conversation>.Filter.Eq(c => c.Id, existingConversation.Id);

            var update = Builders<Conversation>.Update
                .Set(c => c.Participants, existingConversation.Participants)
                .Set(c => c.IsGroup, existingConversation.IsGroup)
                .Set(c => c.GroupName, existingConversation.GroupName)
                .Set(c => c.GroupImageUrl, existingConversation.GroupImageUrl)
                .Set(c => c.CreatedAt, existingConversation.CreatedAt)
                .Set(c => c.LastMessage, existingConversation.LastMessage)
                .Set(c => c.AdminId, existingConversation.AdminId);

            await _conversations.UpdateOneAsync(filter, update);

            // Optionally return the updated document (or re-fetch it)
            return existingConversation;
        }

        public async Task<Message> UpdateLastMessageAsync(Message lastMessage)
        {
            var filter = Builders<Conversation>.Filter.Eq(c => c.Id, lastMessage.ConversationId);

            var update = Builders<Conversation>.Update
                .Set(c => c.LastMessage, lastMessage);

            await _conversations.UpdateOneAsync(filter, update);

            return lastMessage;
        }

        public async Task<Conversation> GetConversationByIdAsync(string id)
        {
            return await _conversations.Find(c => c.Id == id)
                                  .FirstOrDefaultAsync();
        }

        public async Task<List<Conversation>> GetConversationsAsync(string userId, string? next, int pageSize)
        {
            var filter = Builders<Conversation>.Filter.AnyEq(c => c.Participants, userId);

            if (!string.IsNullOrEmpty(next) && DateTime.TryParse(next, out var nextDate))
            {
                filter &= Builders<Conversation>.Filter.Lte(c => c.LastMessage.SentAt, nextDate);
            }

            var conversations = _conversations.Find(filter).SortByDescending(c => c.LastMessage.SentAt).Limit(pageSize);

            return await conversations.ToListAsync();
        }

        public async Task RemoveAsync(string conversationId)
        {
            
            await _conversations.DeleteOneAsync(c => c.Id == conversationId);
        }
    }
}
