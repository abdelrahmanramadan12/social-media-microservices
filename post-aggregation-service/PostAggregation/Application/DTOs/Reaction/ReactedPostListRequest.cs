namespace Application.DTOs.Reaction
{
    public class ReactedPostListRequest
    {
        public string UserId { get; set; }
        public List<string> PostIds { get; set; }
    }
}
