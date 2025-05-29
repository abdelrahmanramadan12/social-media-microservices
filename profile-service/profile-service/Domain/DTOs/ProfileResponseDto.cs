using Domain.Entities;

namespace Domain.DTOs
{
    public class ProfileResponseDto
    {
        public bool Success { get; set; }
        public List<string>? Errors { get; set; }
        public Profile profile { get; set; } = new Profile();
    }
}
