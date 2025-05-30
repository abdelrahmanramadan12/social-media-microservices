using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Application.DTO.ReactionPost.Response
{
    public class PagedReactsResponse
    {
        public List<ReactDto> Reactions { get; set; }
        public string? NextCursor { get; set; }  
        public bool HasMore { get; set; }
    }

}
