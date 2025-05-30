using Microsoft.AspNetCore.Http;

namespace Domain.DTOs
{
    public class EditCommentRequestDto
    {
        public string CommentId { get; set; } = default!;
        public string Content { get; set; } = default!;
        public IFormFile? Media { get; set; }
        public string? UserId { get; set; }
    }
}
