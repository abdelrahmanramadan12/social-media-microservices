namespace Application.DTOs
{
    public class GetProfilePostListRequest
    {
        public string UserId { get; set; }
        public string? Next { get; set; }
    }
}