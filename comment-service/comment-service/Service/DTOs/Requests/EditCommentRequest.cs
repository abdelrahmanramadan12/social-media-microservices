using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Service.DTOs.Requests
{
    public class EditCommentRequest
    {
        public string CommentId { get; set; } = default!;
        public string Content { get; set; } = default!;
        public IFormFile? Media { get; set; }
        public string? MediaUrl { get; set; } 
        public string? UserId { get; set; } = default!;
        public bool HasMedia { get; set; }
        public MediaType MediaType { get; set; } = MediaType.UNKNOWN;
    }
}
