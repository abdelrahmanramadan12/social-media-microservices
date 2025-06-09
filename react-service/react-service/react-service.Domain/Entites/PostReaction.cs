using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace react_service.Domain.Entites
{
    public enum ReactionType
    {
        Like = 0,
        Love = 1,
        Haha = 2,
        Wow = 3,
        Sad = 4,
        Angry = 5
    }
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