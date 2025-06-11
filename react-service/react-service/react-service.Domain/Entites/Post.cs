using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace react_service.Domain.Entites
{
    public class Post
    {
        [BsonId]
        public string PostId { get; set; }
        public string AuthorId { get; set; }
        [BsonElement("isDeleted")]
        public bool IsDeleted { get; set; }
    }
}