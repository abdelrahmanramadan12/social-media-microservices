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
            EnsureIndexes();
        }

        private void EnsureIndexes()
        {
            var indexKeys = Builders<Message>.IndexKeys.Ascending(m => m.ConversationId);
            var indexModel = new CreateIndexModel<Message>(indexKeys);
            _messages.Indexes.CreateOne(indexModel);
        }

        public async Task<Message> AddAsync(Message messageEntity)
        {
            await _messages.InsertOneAsync(messageEntity);
            return messageEntity;
        }

        public async Task<Message> EditAsync(Message existingMessage)
        {
            var filter = Builders<Message>.Filter.Eq(m => m.Id, existingMessage.Id);
            var res = await _messages.ReplaceOneAsync(filter,existingMessage);
            if (res.IsAcknowledged && res.ModifiedCount > 0)
            {
                var msg = _messages.Find(filter);
                return await msg.FirstOrDefaultAsync();
            }
            return null;
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

        public async Task MarkReadAsync(string userId, string conversationId)
        {
            var filter = Builders<Message>.Filter.And(
                Builders<Message>.Filter.Eq(m => m.ConversationId, conversationId),
                Builders<Message>.Filter.Not(Builders<Message>.Filter.Exists($"ReadBy.{userId}"))
            );

            var update = Builders<Message>.Update.Set($"ReadBy.{userId}", DateTime.UtcNow);

            await _messages.UpdateManyAsync(filter, update);
        }

        public async Task RemoveAsync(string messageId)
        {
            var filter = Builders<Message>.Filter.Eq(m => m.Id, messageId);
            await _messages.DeleteOneAsync(filter);
        }

        public async Task DeleteMessagesByConversationIdAsync(string conversationId)
        {
            var filter = Builders<Message>.Filter.Eq(m => m.ConversationId, conversationId);
            await _messages.DeleteOneAsync(filter);
        }
    }
}
