using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public class Follow
    {
        public ObjectId Id { get; set; }
        [BsonElement("follower_id")]
        public string FollowerId { get; set; }
        [BsonElement("following_id")]
        public string FollowingId { get; set; }
        [BsonElement("followed_at")]
        public DateTime FollowedAt { get; set; }
    }
}
