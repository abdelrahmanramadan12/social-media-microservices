using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Service.DTOs
{
    public class MediaUploadRequestDto
    {
        public IFormFile File { get; set; } = null!;
        public UsageCategory usageCategory { get; set; } = UsageCategory.Comment;
        public MediaType MediaType { get; set; }

    }
}
