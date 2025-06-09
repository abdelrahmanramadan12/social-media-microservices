using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Profile
{
    public class SimpleUserProfile
    {
        public string UserId { get; set; }
        public string? DisplayName { get; set; }
        public string? UserName { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
