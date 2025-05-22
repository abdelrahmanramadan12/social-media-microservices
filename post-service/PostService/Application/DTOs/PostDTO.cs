using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{

    public class PostDTO
    {
        public string PostId { get; set; }
        public string userId { get; set; }
        public string Content { get; set; }
        public Privacy Privacy { get; set; }
        public bool HasMedia { get; set; } 
        public IFormFile? Media { get; set; }
    }
}
