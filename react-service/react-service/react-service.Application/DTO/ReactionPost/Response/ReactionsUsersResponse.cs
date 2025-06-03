using System.Collections.Generic;

namespace react_service.Application.DTO.ReactionPost.Response
{
    public class ReactionsUsersResponse
    {
        public List<string> UserIds { get; set; }
        public string Next { get; set; }
        public bool HasMore { get; set; }
    }
}
