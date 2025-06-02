using Microsoft.AspNetCore.Http;

namespace Service.DTOs
{
    public class ProfileRequestDto
    {
        public string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Address { get; set; }
        public DateTime BirthDate { get; set; }
        public string? MobileNo { get; set; }
        public string? Bio { get; set; }
        public IFormFile? ProfilePic { get; set; }
        public IFormFile? CoverPic { get; set; }

    }
}
