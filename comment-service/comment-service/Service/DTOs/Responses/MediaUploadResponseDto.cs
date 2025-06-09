using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTOs.Responses
{
    public class MediaUploadResponseDto
    {
        public bool Success { get; set; }
        public int Uploaded { get; set; }
        public List<string> Urls { get; set; }
    }
}
