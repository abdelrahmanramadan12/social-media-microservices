namespace Application.DTOs
{
    public class ConversationsPageRequestDTO
    {
        public string UserId { get; set; } = string.Empty;
        public int PageSize { get; set; } = 20;
        public string? Next { get; set;}
    }
}
