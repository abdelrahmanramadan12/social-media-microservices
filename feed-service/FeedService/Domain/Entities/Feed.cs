using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public class Feed
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("userId")]
        public string UserId { get; set; }
        [BsonElement("timeline")]
        public List<Post> Timeline { get; set; }
    }
}
