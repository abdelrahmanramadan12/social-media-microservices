using Application.Abstractions;
using Domain.Entities;
using MongoDB.Driver;

namespace Infrastructure.Repositories
{
    public class MessagesRepository : IMessageRepository
    {
        private readonly IMongoCollection<Message> _messages;

        public MessagesRepository(IMongoDatabase db)
        {
            _messages = db.GetCollection<Message>("messages");
        }

        public async Task<Message> AddAsync(Message messageEntity)
        {
            await _messages.InsertOneAsync(messageEntity);
            return messageEntity;
        }

        public async Task<Message> GetByIdAsync(string messageId)
        {
            var filter = Builders<Message>.Filter.Eq(m => m.Id, messageId);
            var messages = _messages.Find(filter);
            return await messages.FirstOrDefaultAsync();
        }

        public async Task<List<Message>> GetMessagesPageAsync(string conversationId, string? next, int pageSize)
        {
            var filter = Builders<Message>.Filter.Eq(m => m.ConversationId, conversationId);

            if (!string.IsNullOrEmpty(next) && DateTime.TryParse(next, out var nextDate))
            {
                filter &= Builders<Message>.Filter.Lte(m => m.SentAt, nextDate);
            }

            var messages = _messages.Find(filter).SortByDescending(m => m.SentAt).Limit(pageSize);

            return await messages.ToListAsync();
        }

        public async Task RemoveAsync(string messageId)
        {
            var filter = Builders<Message>.Filter.Eq(m => m.Id, messageId);
            await _messages.DeleteOneAsync(filter);
        }
    }
}
