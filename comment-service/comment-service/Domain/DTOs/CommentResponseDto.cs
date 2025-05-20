using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class CommentResponseDto
    {
        public string CommentId { get; set; } = default!;
        public string PostId { get; set; } = default!;
        public string AuthorId { get; set; } = default!;
        public string CommentContent { get; set; } = default!;
        public string? MediaUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsEdited { get; set; }
    }
}
