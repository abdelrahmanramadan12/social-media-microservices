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
        [BsonId]
        public string Id { get; set; }
        public string AuthorId { get; set; }
        public Privacy Privacy { get; set; }

    }
}
