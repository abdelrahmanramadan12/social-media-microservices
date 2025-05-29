namespace Application.DTOs.Profile
{
    public class SingleProfileResponse
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
        public PostAuthorProfile PostAuthorProfile { get; set; }
    }
}
