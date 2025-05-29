using MongoDB.Bson;

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
        public ObjectId PostId { get; set; }
        public string Content { get; set; }
        public List<MediaItem> MediaList { get; set; }
        public int ReactsCount { get; set; }
        public int CommentsCount { get; set; }
        public bool IsEdited { get; set; }
        public bool IsLiked { get; set; }
        public DateTime CreatedAt { get; set; }
        public AuthorProfile AuthorProfile { get; set; }
        public Privacy Privacy { get; set; }
    }
}