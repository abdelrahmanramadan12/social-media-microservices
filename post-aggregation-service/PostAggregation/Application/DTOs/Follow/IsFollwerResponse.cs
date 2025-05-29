namespace Application.DTOs.Follow
{
    public class IsFollwerResponse
    {
        public bool IsFollwer { get; set; }
        public List<string> Errors { get; set; }
        public bool success { get; set; }
    }
}