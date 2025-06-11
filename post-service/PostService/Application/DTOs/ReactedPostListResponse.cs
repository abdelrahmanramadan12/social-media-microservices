namespace Application.DTOs
{
    public class ReactedPostListResponse
    {
        public bool Success { get; set; }
        public List<string> ReactedPosts { get; set; }
        public List<string> Errors { get; set; }
    }
}
