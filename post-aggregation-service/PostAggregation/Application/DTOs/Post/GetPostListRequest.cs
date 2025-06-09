namespace Application.DTOs.Post
{
    public class GetPostListRequest
    {
        public string UserId { get; set; }
        public List<string> PostIds { get; set; }
    }
} 