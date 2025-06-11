using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using react_service.Domain.Enums;
namespace react_service.Domain.Entites
{
    public class PostReaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("postId")]
        public string? PostId { get; set; }

        [BsonElement("userId")]
        public string? UserId { get; set; }

        [BsonElement("reactionType")]
        public ReactionType ReactionType { get; set; }
        [BsonElement("isDeleted")]
        public bool IsDeleted { get; set; } = false;

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; }
    }
}