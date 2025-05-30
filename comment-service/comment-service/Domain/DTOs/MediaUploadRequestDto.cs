using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Domain.DTOs
{
    public class MediaUploadRequestDto
    {
        public IEnumerable<IFormFile> Files { get; set; } = null!;
        public UsageCategory usageCategory { get; set; } = UsageCategory.Comment;
        public MediaType MediaType { get; set; }

    }
}
