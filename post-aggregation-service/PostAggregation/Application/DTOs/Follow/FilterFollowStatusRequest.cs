namespace Application.DTOs.Follow
{
    public class FilterFollowStatusRequest
    {
        public string UserId { get; set; }
        public List<string> OtherIds { get; set; } = new List<string>();
    }
}