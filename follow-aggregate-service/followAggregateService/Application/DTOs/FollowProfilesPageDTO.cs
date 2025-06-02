namespace Application.DTOs
{
    public class FollowProfilesPageDTO
    {
        public List<ProfileDTO> Profiles { get; set; }
        public string? Next { get; set; }
    }
}
