using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ReactedPostListResponse
    {
        public bool Success { get; set; }
        public List<string> ReactedPosts { get; set; }
        public List<string> Errors { get; set; }
    }
}
