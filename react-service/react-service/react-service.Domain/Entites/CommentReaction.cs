using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using react_service.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Domain.Entites
{
    public class CommentReaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("commentId")]
        public string? CommentId { get; set; }

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
