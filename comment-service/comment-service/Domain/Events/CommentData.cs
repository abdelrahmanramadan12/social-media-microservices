namespace Domain.Events
{
    public class CommentData
    {
        public string CommentId { get; set; } = default!;
        public string PostId { get; set; } = default!;
        public string? Content { get; set; }
        public string? CommentAuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? PostAuthorId { get; set; }
    }
}
