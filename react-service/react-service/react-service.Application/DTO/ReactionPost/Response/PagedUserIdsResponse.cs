using System.Collections.Generic;

namespace react_service.Application.DTO.ReactionPost.Response
{
    public class PagedUserIdsResponse
    {
        public List<string> UserIds { get; set; }
        public string? Next { get; set; }
        public bool HasMore { get; set; }
    }
}
