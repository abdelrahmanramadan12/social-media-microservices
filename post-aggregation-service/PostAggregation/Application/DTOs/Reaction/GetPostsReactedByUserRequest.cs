namespace Application.DTOs.Reaction
{
    public class GetPostsReactedByUserRequest
    {
        public string UserId { get; set; }
        public List<string> PostIds { get; set; }
    }
} 