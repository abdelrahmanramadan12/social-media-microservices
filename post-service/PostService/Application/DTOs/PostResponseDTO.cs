using Domain.Enums;

namespace Application.DTOs
{
    public class PostResponseDTO
    {
        public string AuthorId { get; set; }
        public string PostId { get; set; }
        public string PostContent { get; set; }
        public Privacy Privacy { get; set; }
        public List<MediaDTO> Media { get; set; } = new List<MediaDTO>();
        public bool HasMedia => Media.Count > 0;
        public DateTime CreatedAt { get; set; }
        public bool IsEdited { get; set; }
        public int NumberOfLikes { get; set; }
        public int NumberOfComments { get; set; }
    }
}
