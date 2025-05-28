using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CacheEntities.Comments
{
    public class CachedComments
    {
        public string UserId { get; set; } = string.Empty; // Unique identifier for the user who made the comment
        public List<CommnetDetails> CommnetDetails { get; set; } = []; // Details of the comment, including comment ID and content
    }
}
