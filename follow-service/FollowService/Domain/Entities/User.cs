using MongoDB.Bson;

namespace Domain.Entities
{
    public class User
    {
        public ObjectId Id { get; set; }
        public string UserId { get; set; }
        public ICollection<string> Followers { get; set; }
        public ICollection<string> Following {  get; set; }
    }
}
