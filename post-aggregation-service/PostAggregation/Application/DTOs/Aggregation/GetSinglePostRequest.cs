using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Aggregation
{
    public class GetSinglePostRequest
    {
        public string PostId { get; set; }
        public string UserId { get; set; }
    }
}
