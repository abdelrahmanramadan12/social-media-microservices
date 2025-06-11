using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Service.DTOs.Requests
{
    public class CreateCommentRequest
    {
        public string PostId { get; set; } = default!;
        public string Content { get; set; } = default!;
        public IFormFile? Media { get; set; }
        public string? UserId { get; set; }
        public bool HasMedia { get; set; }
        public MediaType MediaType { get; set; }
    }
}
