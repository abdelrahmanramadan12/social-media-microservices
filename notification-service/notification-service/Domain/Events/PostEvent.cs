using Domain.CacheEntities;

namespace Domain.Events
{
    public class PostEvent
    {
        public string PostId { get; set; } = string.Empty; // Unique identifier for the post
        public string PostContent { get; set; } = string.Empty; // Unique identifier for the user who created the post
        public UserSkeleton UserMostUsedData { get; set; } = null!;// Data about the user who created the post
    }
}
