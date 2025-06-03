namespace react_service.Application.DTO.ReactionPost.Request
{
    public class FilterPostsReactedByUserRequest
    {
        public List<string>? PostIds { get; set; }
        public string? UserId { get; set; }
    }
}