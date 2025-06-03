namespace Services.DTOs
{
    public class ListFollowPageRequest
    {
        public required string UserId { get; set; }
        public string? Next { get; set; }
    }
}
