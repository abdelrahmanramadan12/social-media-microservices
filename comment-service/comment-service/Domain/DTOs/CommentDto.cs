namespace Domain.DTOs
{
    public class CommentDto
    {
        public string CommentId { get; set; } = default!;
        public string PostId { get; set; } = default!;
        public string AuthorId { get; set; } = default!;
        public string CommentContent { get; set; } = default!;
        public string? MediaUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ReactCount { get; set; }
        public bool IsLiked { get; set; }
        public bool IsEdited { get; set; }
    }
}
