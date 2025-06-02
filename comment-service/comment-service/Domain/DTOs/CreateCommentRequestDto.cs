using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Domain.DTOs
{
    public class CreateCommentRequestDto
    {
        public string PostId { get; set; } = default!;
        public string Content { get; set; } = default!;
        public string? UserId { get; set; }
        public bool HasMedia { get; set; }
        public MediaType MediaType { get; set; }
        public IFormFile? Media { get; set; }
    }
}
