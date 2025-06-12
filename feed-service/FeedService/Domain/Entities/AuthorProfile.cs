using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public class AuthorProfile
    {
        [BsonElement("id")]
        public string Id { get; set; }
        [BsonElement("userName")]
        public string UserName { get; set; }
        [BsonElement("displayName")]
        public string DisplayName { get; set; }
        [BsonElement("profilePictureUrl")]
        public string ProfilePictureUrl { get; set; }
    }
}
