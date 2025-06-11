namespace Application.DTOs
{
    public class GetProfilePostListRequest
    {
        public string UserId { get; set; }
        public string? Next { get; set; }
        public string? ProfileUserId { get; set; }
    }
}