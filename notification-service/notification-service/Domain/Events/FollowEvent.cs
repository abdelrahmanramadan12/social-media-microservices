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
        public string FollowingId { get; set; } = string.Empty; // Unique identifier for the user who made the comment
        public string FollowerId { get; set; } = string.Empty; // Unique identifier for the user who reacted
        public DateTime Timestamp { get; set; } // Timestamp of when the reaction was created


    }

}
