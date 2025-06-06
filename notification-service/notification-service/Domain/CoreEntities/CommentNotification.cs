using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.CoreEntities
{
    public class CommentNotification
    {
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string PostAuthorId { get; set; } = string.Empty;
        public string PostId { get; set; } = string.Empty;
        public Dictionary<string, List<string>> UserID_CommentIds { get; set; } = [];

        public List<string> CommentNotifReadByAuthor { get; set; } = [];

    }
}
