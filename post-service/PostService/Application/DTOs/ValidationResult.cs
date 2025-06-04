namespace Application.DTOs
{
    public class ValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<string> Errors { get; set; } = new();
        public ErrorType ErrorType { get; set; } = ErrorType.None;
    }
}
