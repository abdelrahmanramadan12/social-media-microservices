using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Application.DTO.ReactionPost.Request
{
    public class GetReactsOfPostRequest
    {
        public string PostId { get; set; }
        public string? NextReactIdHash { get; set; }
    }
}