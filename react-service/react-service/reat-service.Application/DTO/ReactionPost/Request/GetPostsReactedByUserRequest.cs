namespace reat_service.Application.DTO.ReactionPost.Request
{
    public class GetPostsReactedByUserRequest
    {
        public List<string> PostIds { get; set; }
        public string UserId { get; set; }
    }
}