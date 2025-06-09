namespace Application.DTOs
{
    public class MessagesPageRequestDTO
    {
        public string ConversationId { get; set; } = string.Empty;
        public int PageSize { get; set; } = 20;
        public string? Next { get; set; }
    }
}
