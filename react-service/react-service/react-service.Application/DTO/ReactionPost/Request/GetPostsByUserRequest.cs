using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Application.DTO.ReactionPost.Request
{
    public class GetPostsByUserRequest
    {
        public string UserId { get; set; }
        public string? Next { get; set; }
    }
}