using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{

    public class PostInputDTO
    {
        public string PostId { get; set; }
        public string AuthorId { get; set; }
        public string Content { get; set; }
        public Privacy Privacy { get; set; }
        public List<string> MediaUrls { get; set; } = new List<string>();
        public bool HasMedia { get; set; }
        public MediaType MediaType { get; set; }
        public IEnumerable<IFormFile> Media { get; set; }
    }
}
