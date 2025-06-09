using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using react_service.Domain.Entites;
using react_service.Domain.Enums;

namespace react_service.Application.DTO
{
    public  class ReactionResponseDTO
    {         
        public string? PostId { get; set; }          
        public ReactionType ReactionType { get; set; }
    }
}
