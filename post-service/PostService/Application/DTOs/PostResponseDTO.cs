using Domain.Enums;

namespace Application.DTOs
{
    public class PostResponseDTO
    {
        public string AuthorId { get; set; }
        public string PostId { get; set; }
        public string PostContent { get; set; }
        public Privacy Privacy { get; set; }
        public List<string> MediaUrls { get; set; } = new List<string>();
        public bool HasMedia => MediaUrls.Count() > 0;
        public DateTime CreatedAt { get; set; }
        public bool IsEdited { get; set; }
        public int NumberOfLikes { get; set; }
        public int NumberOfComments { get; set; }
    }
}
