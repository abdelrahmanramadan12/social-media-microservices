namespace Application.DTO
{

    public enum ErrorType
    {
        None,
        NotFound,
        BadRequest,
        UnAuthorized,
        Validation,
        InternalServerError
    }

    public class PaginationMetadata
    {
        public string? Next { get; set; }
        public bool HasMore => !string.IsNullOrEmpty(Next);
    }

    public class ResponseWrapper<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public PaginationMetadata? Pagination { get; set; }

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
