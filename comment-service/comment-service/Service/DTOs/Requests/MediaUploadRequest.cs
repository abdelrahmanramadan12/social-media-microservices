using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Requests
{
    public class MediaUploadRequestDto
    {
        public IFormFile File { get; set; } = null!;
        public UsageCategory usageCategory { get; set; } = UsageCategory.Comment;
        public MediaType MediaType { get; set; }

    }
}
