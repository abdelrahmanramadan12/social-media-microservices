namespace Application.DTOs.Follow
{
    public class IsFollowerRequest
    {
        public string userId { get; set; }
        public string targetUserId { get; set; }
    }
}
