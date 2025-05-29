namespace Application.DTOs.Post
{
    public class ProfilePostsResponse
    {
        public bool Success { get; set; }
        public string NextPostHashId { get; set; }
        public List<Post> Posts { get; set; }
        public List<String> Errors { get; set; }
    }
}
