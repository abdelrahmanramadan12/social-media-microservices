namespace Domain.Events
{
    public enum CommentEventType
    {
        Created,
        Deleted
    }
    public class CommentEvent
    {
        public CommentEventType EventType { get; set; }
        public CommentData? Data { get; set; }
    }
}
