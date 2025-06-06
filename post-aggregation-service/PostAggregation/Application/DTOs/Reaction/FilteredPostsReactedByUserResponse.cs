using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reaction
{
    public class FilteredPostsReactedByUserResponse
    {
        public List<string> PostIds { get; set; } = new List<string>();
    }
}
