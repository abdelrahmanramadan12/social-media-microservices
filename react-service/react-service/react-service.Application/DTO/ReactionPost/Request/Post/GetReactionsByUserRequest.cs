using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Application.DTO.ReactionPost.Request.Post
{
    public class GetReactionsByUserRequest
    {
        public string UserId { get; set; }
        public string Next { get; set; }
    }
}
