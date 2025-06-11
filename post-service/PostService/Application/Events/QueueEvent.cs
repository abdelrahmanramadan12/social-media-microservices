namespace Application.Events
{
    public enum EventType
    {
        Create,
        Update,
        Delete,
        Follow,
        Unfollow
    }
    public class QueueEvent
    {
        public EventType EventType { get; set; }
    }
}
