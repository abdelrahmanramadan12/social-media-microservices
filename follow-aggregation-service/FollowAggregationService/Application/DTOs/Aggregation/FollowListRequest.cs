using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Aggregation
{
    public class FollowListRequest
    {
        public string? UserId { get; set; }
        public string OtherId { get; set; }
        public string? Next {  get; set; }
    }
}
