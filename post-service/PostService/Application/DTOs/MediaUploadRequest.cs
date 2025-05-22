using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
