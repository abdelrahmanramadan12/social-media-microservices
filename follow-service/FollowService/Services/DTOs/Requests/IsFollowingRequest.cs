namespace Services.DTOs.Requests
{
    public class IsFollowingRequest
    {
        public required string UserId { get; set; }
        public required string OtherId { get; set; }
    }
}
