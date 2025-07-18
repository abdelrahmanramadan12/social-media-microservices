namespace react_service.Application.Events
{
    public class CommentEvent : QueueEvent
    {
        public EventType EventType { get; set; }
        public string CommentId { get; set; } = default!;
        public string PostId { get; set; } = default!;
        public string Content { get; set; } = default!;
        public string? MediaUrl { get; set; }
        public string CommentAuthorId { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public string PostAuthorId { get; set; } = default!;
        public bool IsEdited { get; set; }
    }
}
