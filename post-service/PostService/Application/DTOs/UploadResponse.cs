using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class UploadResponse
    {
        public bool Success { get; set; }
        public int Uploaded { get; set; }
        public List<string> Urls { get; set; }
    }
}
