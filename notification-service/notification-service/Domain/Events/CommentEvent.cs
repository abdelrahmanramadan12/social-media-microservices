using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events
{
    public class CommentEvent
    {
        public string Id { get; set; }
        public string CommentorId { get; set; } 
        public string PostId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string  PostAuthorId { get; set; }
    }
}
