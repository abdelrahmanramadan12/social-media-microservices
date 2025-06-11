using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reaction
{
    public class FilterCommentsReactedByUserRequest
    {
        public List<string>? CommentIds { get; set; }
        public string? UserId { get; set; }
    }
}
