namespace Application.Events
{
    public class CommentEvent : QueueEvent
    {
        public EventType EventType { get; set; }
        public string CommentId { get; set; }
        public string Content { get; set; }
        public string? MediaUrl { get; set; }
        public DateTime Timestamp { get; set; }
        public string CommentAuthorId { get; set; }
        public string PostId { get; set; }
        public string PostAuthorId { get; set; }
        public bool IsEdited { get; set; }
    }
}
