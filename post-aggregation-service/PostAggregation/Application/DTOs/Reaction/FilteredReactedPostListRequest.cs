namespace Application.DTOs.Reaction
{
    public class FilteredReactedPostListRequest
    {
        public string UserId { get; set; }
        public List<string> PostIds { get; set; }
    }
}
