using Microsoft.AspNetCore.Http;

namespace Service.DTOs.Requests
{
    public class CreateCommentRequest
    {
        public string PostId { get; set; } = default!;
        public string Content { get; set; } = default!;
        public IFormFile? Media { get; set; }
        public string? UserId { get; set; }
    }
}
