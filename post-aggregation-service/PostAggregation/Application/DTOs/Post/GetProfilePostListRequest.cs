namespace Application.DTOs.Post
{
    public class GetProfilePostListRequest
    {
        public string UserId { get; set; }
        public string? Next { get; set; }
        public string? ProfileUserId { get; set; }
    }
} 