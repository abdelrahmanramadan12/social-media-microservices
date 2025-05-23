using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class PostAuthorProfile
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
