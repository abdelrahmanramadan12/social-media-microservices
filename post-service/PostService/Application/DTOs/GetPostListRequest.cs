using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class GetPostListRequest
    {
        public string UserId { get; set; }
        public List<string> PostIds { get; set; }
    }
}