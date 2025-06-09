using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Follow
{
    public class FilterFollowStatusRequest
    {
        public string UserId { get; set; }
        public List<string> OtherIds { get; set; } = new List<string>();
    }
}
