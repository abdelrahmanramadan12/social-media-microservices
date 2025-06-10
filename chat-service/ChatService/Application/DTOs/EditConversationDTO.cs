using Microsoft.AspNetCore.Http;

namespace Application.DTOs
{
    public class EditConversationDTO
    {
        public string Id { get; set; } = string.Empty;
        public List<string> Participants { get; set; } = [];
        public bool IsGroup { get; set; }
        public string? UserId { get; set; }
        public string? GroupName { get; set; }
        public IFormFile? GroupImage { get; set; }
    }
}
