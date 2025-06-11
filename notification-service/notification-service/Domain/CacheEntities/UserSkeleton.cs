namespace Domain.CacheEntities
{
    public class UserSkeleton
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty; // Unique identifier for the user who reacted
        public string ProfileImageUrls { get; set; } = string.Empty; // URL of the profile image of the user who reacted
        public string UserNames { get; set; } = string.Empty; // Name of the user who reacted
        public bool Seen { get; set; }
        public DateTime CreatedAt { get; set; } // Timestamp of when the reaction was created

    }
}
