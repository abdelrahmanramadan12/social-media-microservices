using Domain.Entities;

namespace Application.DTOs
{
    public class ConversationDTO
    {
        public string Id { get; set; }
        public List<string>? Participants { get; set; }
        public bool IsGroup { get; set; }
        public string? AdminId { get; set; } 
        public string? GroupName { get; set; }
        public string? GroupImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public MessageDTO? LastMessage { get; set; }
    }
}
