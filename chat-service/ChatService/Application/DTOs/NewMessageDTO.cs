using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs
{
    public class NewMessageDTO
    {
        public string ConversationId { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string? Content { get; set; }
        public IFormFile? Media { get; set; }
        public AttachmentType? MediaType { get; set; }
    }
}
