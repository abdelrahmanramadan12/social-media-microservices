using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs
{
    public class ReceivedMediaDto
    {
        public IEnumerable<IFormFile> Files { get; set; } = null!;
        public MediaType MediaType { get; set; }
        public UsageCategory UsageCategory { get; set; }
    }
}
