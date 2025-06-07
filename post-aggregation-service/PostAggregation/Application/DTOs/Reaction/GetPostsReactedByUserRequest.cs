namespace Application.DTOs.Reaction
{
    public class GetPostsReactedByUserRequest
    {
        public string UserId { get; set; }
        public string Next { get; set; }
    }
}
