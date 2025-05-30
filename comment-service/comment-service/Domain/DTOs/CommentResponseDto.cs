namespace Domain.DTOs
{
    public class CommentResponseDto
    {
        public bool Success { get; set; }
        public List<string>? Messages { get; set; }
        public CommentDto? Data { get; set; }
    }
}
