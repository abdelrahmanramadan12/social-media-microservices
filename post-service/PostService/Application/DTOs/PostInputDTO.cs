using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Application.DTOs
{
    public class PostInputDTO
    {
        public string PostId { get; set; }
        public string AuthorId { get; set; }
        public string Content { get; set; }
        public Privacy Privacy { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MediaType MediaType { get; set; }
        
        [JsonPropertyName("MediaUrls")]
        public List<string> MediaUrls { get; set; } = new List<string>();
        
        public bool HasMedia { get; set; }
        
        [JsonPropertyName("Media")]
        public IEnumerable<IFormFile> Media { get; set; }
    }
}
