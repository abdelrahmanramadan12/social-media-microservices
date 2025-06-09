namespace Application.DTOs
{
    public class ConversationsPageRequestDTO
    {
        public int PageSize { get; set; } = 20;
        public string? Next { get; set;}
    }
}
