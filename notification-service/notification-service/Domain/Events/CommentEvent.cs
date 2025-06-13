namespace Domain.Events
{
    public enum EventType
    {
        Create,
        Update,
        Delete
    }
    public class CommentEvent
    {
        public string CommentId { get; set; } = string.Empty;
        public string CommentAuthorId { get; set; } = default!;
        public string PostId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string PostAuthorId { get; set; } = string.Empty;
        public EventType EventType { get; set; }
        public bool IsEdited { get; set; }
        public string? MediaUrl { get; set; }

    }
}
