namespace Service.Events
{
    public class CommentEvent
    {
        public CommentEventType EventType { get; set; }
        public CommentData Data { get; set; } = new();
    }

    public class CommentData
    {
        public string CommentId { get; set; } = default!;
        public string PostId { get; set; } = default!;
        public string CommentAuthorId { get; set; } = default!;
        public string? Content { get; set; }
        public string? MediaUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PostAuthorId { get; set; } = default!;
        public bool IsEdited { get; set; }
    }

    public class CommentCreatedEvent
    {
        public string PostId { get; set; } = default!;
        public string Content { get; set; } = default!;
        public string? MediaURL { get; set; }
        public string CommentAuthorId { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public string PostAuthorId { get; set; } = default!;
        public bool IsEdited { get; set; }
    }

    public class CommentDeletedEvent
    {
        public string PostId { get; set; } = default!;
    }
}
