namespace Domain.DTOs
{
    public class FollowsPageDTO
    {
        public ICollection<string>? Follows { get; set; }
        public int? Next { get; set; }
    }
}
