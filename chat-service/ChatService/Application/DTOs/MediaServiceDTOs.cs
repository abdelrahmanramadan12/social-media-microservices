using Microsoft.AspNetCore.Http;

namespace Application.DTOs
{
    public class ReceivedMediaDTO
    {
        public IFormFileCollection Files { get; set; }
        public string MediaType { get; set; }
    }
} 