namespace Application.DTOs.Profile
{
    public class SimpleUserProfile
    {
        public string UserId { get; set; }
        public string? DisplayName { get; set; }
        public string? UserName { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}