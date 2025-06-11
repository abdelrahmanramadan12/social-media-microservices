namespace react_service.Application.DTO
{
    public class PaginationResponseWrapper<T>
    {
        public T Data { get; set; } = default!;
        public bool HasMore { get; set; }
        public string Next { get; set; } = string.Empty;
        public string Message { get; set; }

        private List<string> _errors = new List<string>();
        public List<string> Errors
        {
            get => _errors;
            set => _errors = value ?? new List<string>();
        }
        public bool Success => Errors == null || Errors.Count == 0;
        public ErrorType ErrorType { get; set; } = ErrorType.None;
    }
}
