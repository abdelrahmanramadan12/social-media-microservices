namespace Domain.Events
{
    public enum CommentType
    {
        ADDED,
        REMOVED
    }
    public class CommentEvent
    {
        public string Id { get; set; } = string.Empty;
        public string CommentAuthorId { get; set; } = default!;
        public string PostId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string PostAuthorId { get; set; } = string.Empty;
        public CommentType CommentType { get; set; }
    }
}
