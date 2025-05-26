using MongoDB.Bson;

namespace Domain.Entities
{
    public class Feed
    {
        public ObjectId Id { get; set; }
        public string UserId { get; set; }
        public ICollection<Post> Timeline { get; set; }
    }
}
