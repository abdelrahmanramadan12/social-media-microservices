using MongoDB.Bson;

namespace Domain.Entities
{
    public class Follow
    {
        public ObjectId Id { get; set; }
        public string FollowerId { get; set; }
        public string FollowingId { get; set; }
        public DateTime FollowedAt { get; set; }
    }
}
