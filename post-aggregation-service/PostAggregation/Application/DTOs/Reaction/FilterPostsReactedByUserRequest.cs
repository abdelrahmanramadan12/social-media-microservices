namespace Application.DTOs.Reaction
{
    public class FilterPostsReactedByUserRequest
    {
        public string UserId { get; set; }
        public List<string> PostIds { get; set; }
    }
} 