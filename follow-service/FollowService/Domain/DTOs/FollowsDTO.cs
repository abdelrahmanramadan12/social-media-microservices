namespace Domain.DTOs
{
    public class FollowsDTO
    {
        public bool? IsCelebrity { get; set; }
        public ICollection<string> Follows { get; set; }
    }
}
