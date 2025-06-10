namespace Application.DTOs
{
    public class ReactedPostListRequest
    {
        public string UserId { get; set; }
        public List<string> PostIds { get; set; }
    }
}