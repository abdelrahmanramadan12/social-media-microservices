using Domain.Enums;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Post
    {
        public string Id { get; set; }
        public Privacy Privacy { get; set; }
        public string Content { get; set; }
        public string AuthourId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public int NumberOfLikes { get; set; }  
        public int NumberOfComments { get; set; }
        public List<Media> MediaList { get; set; } 

        // Media Property

    }
}
