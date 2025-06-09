namespace react_service.Application.DTO.ReactionPost.Request.Post
{
    public class IsPostLikedByUserRequest
    {
        public string PostId { get; set; }
        public string UserId { get; set; }
    }
}