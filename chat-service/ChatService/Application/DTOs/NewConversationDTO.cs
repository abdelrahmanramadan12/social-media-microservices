namespace Application.DTOs
{
    public class NewConversationDTO
    {
        public List<string> Participants { get; set; } = [];
        public bool IsGroup { get; set; }
        public string? UserId { get; set; }
        public string? GroupName { get; set; }
    }
}
