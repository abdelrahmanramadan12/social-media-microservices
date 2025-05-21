using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class PagedCommentsDto
    {
        public IEnumerable<CommentResponseDto>? Comments { get; set; } 
        public string? NextCommentIdHash { get; set; }  
    }
}
