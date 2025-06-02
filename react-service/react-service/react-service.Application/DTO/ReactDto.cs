using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using react_service.Domain.Entites;

namespace react_service.Application.DTO
{
    public  class ReactDto
    {
        public string? PostId { get; set; }         
        public string? UserId { get; set; }          
        public ReactionType ReactionType { get; set; }
        public string? UserName { get; set; }    
        public string? ProfileUrl { get; set; }  
       
    }
}
