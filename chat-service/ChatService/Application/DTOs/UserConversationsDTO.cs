namespace Application.DTOs
{
    public class UserConversationsDTO
    {
        public List<ConversationDTO>? Conversations { get; set; }
        public string? Next { get; set; }
    }
}