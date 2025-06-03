namespace Domain.Events
{
    public enum FollowEventType
    {
        FOLLOW,
        UNFOLLOW
    }
    public class FollowEvent
    {
        public FollowEventType EventType { get; set; }
        public string UserId { get; set; } = string.Empty; // Unique identifier for the user who made the comment
        public string FollowerId { get; set; } = string.Empty; // Unique identifier for the user who reacted
        public string ProfileImageUrls { get; set; } = string.Empty; // URL of the profile image of the user who reacted
        public string UserNames { get; set; } = string.Empty; // Name of the user who reacted
        public DateTime CreatedAt { get; set; } // Timestamp of when the reaction was created


    }

}
