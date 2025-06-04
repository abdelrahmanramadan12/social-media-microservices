using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.CoreEntities
{
    public class Messages
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string RevieverId { get; set; } = string.Empty;

        public Dictionary<string, List<MessageInfo>> MessageList = [];
        //public List<string> MessageNotifReadByAuthor { get; set; } = []; // List of reaction items, each containing user ID and reaction type

    }

    public class MessageInfo
    {
        public string Id { get; set; } = string.Empty; // Unique identifier for the message
        //public string SenderId { get; set; } = string.Empty;
        public bool IsRead { get; set; }  // Indicates if the message has been read    
        public DateTime SentAt { get; set; }  // Timestamp of when the message was created
    }

}
