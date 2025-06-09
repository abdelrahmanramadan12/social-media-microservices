namespace react_service.Application.DTO.Reaction.Request.Post
{
    public class FilterPostsReactedByUserRequest
    {
        public List<string>? PostIds { get; set; }
        public string? UserId { get; set; }
    }
}