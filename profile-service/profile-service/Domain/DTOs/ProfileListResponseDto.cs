namespace Domain.DTOs
{
    public class ProfileListResponseDto
    {
        public bool Success { get; set; }
        public List<string>? Errors { get; set; }
        public List<SimpleUserDto>? Data { get; set; }
    }
}
