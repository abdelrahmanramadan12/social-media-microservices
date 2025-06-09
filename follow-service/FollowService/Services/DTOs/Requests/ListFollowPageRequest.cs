namespace Services.DTOs.Requests
{
    public class ListFollowPageRequest
    {
        public required string UserId { get; set; }
        public string? Next { get; set; }
    }
}
