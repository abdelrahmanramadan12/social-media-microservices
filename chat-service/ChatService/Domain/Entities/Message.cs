using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string ConversationId { get; set; }
        public string SenderId { get; set; }
        public string Text { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime SentAt { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? EditedAt { get; set; }
        public bool IsEdited { get; set; }
        public Dictionary<string, DateTime>? ReadBy { get; set; }
        public Attachment? Attachment { get; set; }
    }
}
