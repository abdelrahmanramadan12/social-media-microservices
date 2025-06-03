using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public class Conversation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public List<string> Participants { get; set; }
        public bool IsGroup { get; set; }
        public string? GroupName { get; set; }
        public DateTime CreatedAt { get; set; }
        public Message LastMessage { get; set; }
        public string? AdminId { get; set; } // For group conversations, the admin can be null for one-on-one conversations

    }
}
