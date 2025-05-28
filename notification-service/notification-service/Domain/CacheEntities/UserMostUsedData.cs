namespace Domain.CacheEntities
{
    public class UserMostUsedData
    {
        public string UsersId { get; set; } = string.Empty; // Unique identifier for the user who reacted
        public string ProfileImageUrls { get; set; } = string.Empty; // URL of the profile image of the user who reacted
        public string UserNames { get; set; } = string.Empty; // Name of the user who reacted

    }
}
