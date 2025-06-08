using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace react_service.Domain.Entites
{
    public class Post
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PostId { get; set; }
        public string AuthorId { get; set; }
        public bool IsDeleted { get; set; }
    }
}