namespace Domain.DTOs
{
    public class FollowingPageDTO
    {
        public ICollection<string>? Following { get; set; }
        public string? Next { get; set; }
    }
}
