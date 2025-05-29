namespace Domain.Entities
{
    public class Profile
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }

        public string? ProfileUrl { get; set; }
        public string? CoverUrl { get; set; }
        public string? Address { get; set; }

        public DateTime BirthDate { get; set; }
        public string? MobileNo { get; set; }

        public string? Bio { get; set; }

        public int NoFollowers { get; set; }
        public int NoFollowing { get; set; }
    }
}
