using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Application.DTO.Reaction.Response
{
    public class PagedReactsResponse
    {
        public List<ReactionResponseDTO> Reactions { get; set; }
        public string? Next { get; set; }  
        public bool HasMore { get; set; }
    }

}
