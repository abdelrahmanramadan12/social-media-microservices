namespace Domain.DTOs
{
    public class MinProfileResponseDto
    {
        public bool Success { get; set; }
        public List<string>? Errors { get; set; }
        public SimpleUserDto? SimpleUser { get; set; }
    }
}
