namespace Domain.DTOs
{
    public class PagedCommentsDto
    {
        public IEnumerable<CommentDto>? Comments { get; set; }
        public string? Next { get; set; }
    }
}
