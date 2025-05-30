namespace Application.DTOs.Post
{
    public class GetProfilePostListRequest
    {
        public string UserId { get; set; }
        public string? NextCursor { get; set; }
    }
} 