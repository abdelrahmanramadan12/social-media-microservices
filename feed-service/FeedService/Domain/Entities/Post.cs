using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public enum Privacy
    {
        Public,
        Private,
        OnlyMe
    }

    public class Post
    {
        [BsonElement("postId")]
        public string PostId { get; set; }
        [BsonElement("content")]
        public string Content { get; set; }
        [BsonElement("mediaList")]
        public List<MediaItem>? MediaList { get; set; }
        [BsonElement("reactsCount")]
        public int ReactsCount { get; set; }
        [BsonElement("commentsCount")]
        public int CommentsCount { get; set; }
        [BsonElement("isEdited")]
        public bool IsEdited { get; set; }
        [BsonElement("isLiked")]
        public bool IsLiked { get; set; }
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
        [BsonElement("authorProfile")]
        public AuthorProfile AuthorProfile { get; set; }
        [BsonElement("privacy")]
        public Privacy Privacy { get; set; }
    }
}