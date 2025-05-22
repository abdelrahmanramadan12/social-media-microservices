using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class PostResponseDTO
    {
        public string AuthorId {  get; set; }
        public string PostId { get; set; }
        public string PostContent { get; set; }
        public List<string> MediaURL { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsEdited { get; set; }
        public int NumberOfLikes { get; set; }
        public int NumberOfComments { get; set; }
        public bool IsLiked { get; set; }
    }
}
