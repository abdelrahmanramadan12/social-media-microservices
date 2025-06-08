namespace react_service.DTO.ReactionPost.Request
{
    public class IsPostLikedByUserRequest
    {
        public string PostId { get; set; }
        public string UserId { get; set; }
    }
}