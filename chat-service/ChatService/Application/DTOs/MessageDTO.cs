namespace Application.DTOs
{
    public class MessageDTO
    {
        public string Id { get; set; } = string.Empty;
        public string ConversationId { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string? Content { get; set; }
        public bool Read {  get; set; }
    }
}