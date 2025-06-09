using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Aggregation
{
    public class ProfileAggregation
    {
        public string UserId { get; set; }
        public string? DisplayName { get; set; }
        public string? UserName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public bool IsFollowing { get; set; }
        public bool IsFollower { get; set; }
    }
}
