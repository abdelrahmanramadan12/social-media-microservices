using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs
{
    public enum UsageCategory
    {
        ProfilePicture,
        CoverPicture,
        Post,
        Story,
        Message,
        Comment
    }
    public class MediaUploadRequest
    {
        public IEnumerable<IFormFile> Files { get; set; } = null!;
        public UsageCategory usageCategory { get; set; } = UsageCategory.Post;
        public MediaType MediaType { get; set; }
    }
}
