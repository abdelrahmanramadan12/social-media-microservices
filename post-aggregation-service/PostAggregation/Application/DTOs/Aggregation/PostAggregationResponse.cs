using Application.DTOs.Post;
using Application.DTOs.Profile;

namespace Application.DTOs.Aggregation
{
    public class PostAggregationResponse
    {
        public string AuthorId { get; set; }
        public string PostId { get; set; }
        public string PostContent { get; set; }
        public Privacy Privacy { get; set; }
        public List<MediaDTO> Media { get; set; } = new();
        public bool HasMedia => Media.Count() > 0;
        public DateTime CreatedAt { get; set; }
        public bool IsEdited { get; set; }
        public int NumberOfLikes { get; set; }
        public int NumberOfComments { get; set; }
        public bool IsLiked { get; set; }
        public SimpleUserProfile PostAuthorProfile { get; set; }
    }
}
