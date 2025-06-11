using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Follow
{
    public class ListFollowPageRequest
    {
        public string?UserId { get; set; }
        public string? Next { get; set; }
    }
}
