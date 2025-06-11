using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public class User
    {
        [BsonId]
        public string Id { get; set; }
    }
}
