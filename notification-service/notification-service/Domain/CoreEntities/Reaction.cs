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

        // Tracks if the reaction has been seen by the post/comment owner
        public List<string> CommentReactionsNotifReadByAuthor { get; set; } = []; // List of reaction items, each containing user ID and reaction type

        public List<string> PostReactionsNotifReadByAuthor { get; set; } = []; // List of reaction items, each containing user ID and reaction type

    }
}
