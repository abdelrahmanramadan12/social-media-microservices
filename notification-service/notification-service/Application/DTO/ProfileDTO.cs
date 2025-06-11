namespace Application.DTO
{
    public class ProfileDTO
    {
        public string UserId { get; set; } = string.Empty; // Unique identifier for the user who made the comment
        public string ProfileImageUrl { get; set; } = string.Empty; // URL of the profile image of the user who reacted
        public string UserNames{ get; set; } = string.Empty; // Name of the user who reacted

    }
}
