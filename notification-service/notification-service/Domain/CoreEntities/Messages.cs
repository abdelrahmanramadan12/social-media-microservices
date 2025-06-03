using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.CoreEntities
{
    public class Messages
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty; // Unique identifier for the message

        public string SourceUserId { get; set; } = string.Empty;

        public string DestinationUserId { get; set; } = string.Empty;
        public bool IsRead { get; set; }  // Indicates if the message has been read    
        
        public DateTime SentAt{ get; set; }  // Timestamp of when the message was created
    }
}
