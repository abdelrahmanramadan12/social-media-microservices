namespace Application.DTOs.Post
{
    
    public class ProfilePostsRequest
    {
        public string UserId { get; set; }
        public bool IsFollower { get; set; }
        public string NextPostHashId { get; set; }
    }
}
