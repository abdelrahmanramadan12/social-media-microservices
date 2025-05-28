using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.CoreEntities
{
    public class Reaction
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty; // Unique identifier for the reaction
        public string AuthorId { get; set; } = string.Empty;
        public List<string> ReactionsOnCommentId { get; set; } = [];
        public List<string> ReactionsOnPostId { get; set; } = [];

    }
}
