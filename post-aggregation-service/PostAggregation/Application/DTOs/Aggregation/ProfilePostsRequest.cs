using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Aggregation
{
    public class ProfilePostsRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string OtherId { get; set; } = string.Empty;
        public string Next { get; set; } = string.Empty;
    }
}
