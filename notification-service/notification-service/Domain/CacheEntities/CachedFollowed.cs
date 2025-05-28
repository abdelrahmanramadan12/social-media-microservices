namespace Domain.CacheEntities
{
    public class CachedFollowed
    {
        public string UserId { get; set; } = string.Empty; // Unique identifier for the user who made the comment
        public List<UserMostUsedData> Followers { get; set; } = [];

    }
}
