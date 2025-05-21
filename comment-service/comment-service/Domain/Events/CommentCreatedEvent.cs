namespace Domain.Events
{
    public class CommentCreatedEvent
    {
        public string PostId { get; set; } = default!;
        public string? Content { get; set; } 
        public string? MediaURL { get; set; } 
        public string CommentAuthorId { get; set; } = default!;
        public DateTime CreatedAt { get; set; } 
        public string PostAuthorId { get; set; } = default!;
        public bool IsEdited { get; set; }
    }
}
