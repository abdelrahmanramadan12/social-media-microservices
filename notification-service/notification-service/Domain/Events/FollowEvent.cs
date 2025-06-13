namespace Domain.Events
{
    public enum FollowEventType
    {
        Follow,
        Unfollow
    }
    public class FollowEvent
    {
        public FollowEventType EventType { get; set; }
        public string FollowingId { get; set; }
        public string FollowerId { get; set; }
        public DateTime Timestamp { get; set; }


    }

}
