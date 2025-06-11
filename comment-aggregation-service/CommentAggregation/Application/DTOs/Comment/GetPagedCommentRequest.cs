using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Comment
{
    public class GetPagedCommentRequest
    {
        public string PostId { get; set; } = default!;
        public string? UserId { get; set; } = default!;
        public string? Next { get; set; }
    }
}
