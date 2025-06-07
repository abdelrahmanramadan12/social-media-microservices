namespace Application.DTOs
{
    public class ConversationMessagesDTO
    {
        public List<MessageDTO>? Messages { get; set; }
        public string? Next { get; set; }
    }
}
