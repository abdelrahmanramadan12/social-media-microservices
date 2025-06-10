using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reaction
{
    public class IsPostLikedByUserRequest
    {
        public string PostId { get; set; }
        public string UserId { get; set; }
    }
}
