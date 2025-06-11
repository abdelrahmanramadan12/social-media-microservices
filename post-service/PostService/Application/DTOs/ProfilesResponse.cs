namespace Application.DTOs
{
    public class ProfilesResponse
    {
        public bool Success { get; set; }
        public List<PostAuthorProfile> postAuthorProfiles { get; set; }
        public List<string> Errors { get; set; }
    }
}
