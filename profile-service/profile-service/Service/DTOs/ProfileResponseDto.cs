using Domain.Entities;

namespace Service.DTOs
{
    public class ProfileResponseDto
    {
        public bool Success { get; set; }
        public List<string>? Errors { get; set; }
        public Profile Data { get; set; } = new Profile();
    }
}
