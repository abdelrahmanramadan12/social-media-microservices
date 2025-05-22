namespace Domain.DTOs
{
    public class FollowsPageDTO
    {
        public ICollection<string>? Follows { get; set; }
        public string? Next { get; set; }
    }
}
