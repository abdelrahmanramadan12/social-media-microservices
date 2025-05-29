using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserId { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [MaxLength(50)]
        public string? UserName { get; set; }

        [Url]
        [MaxLength(255)]
        public string? ProfileUrl { get; set; }

        [Url]
        [MaxLength(255)]
        public string? CoverUrl { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? MobileNo { get; set; }

        [MaxLength(500)]
        public string? Bio { get; set; }

        public int NoFollowers { get; set; }
        public int NoFollowing { get; set; }
    }
}
