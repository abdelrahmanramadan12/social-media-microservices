namespace Domain.DTOs
{
    public class FollowersPageDTO
    {
        public ICollection<string>? Followers { get; set; }
        public string? Next { get; set; }
    }
}
