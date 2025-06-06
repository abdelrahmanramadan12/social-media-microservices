using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reaction
{
    public class GetReactsOfPostResponse
    {
        public List<string> UserIds { get; set; } = new List<string>();
        public string Next { get; set; } = string.Empty;
        public bool HasMore { get; set; }
    }
}
