using System;
using System.Collections.Generic;

namespace Application.DTOs.Post
{
    public class PostResponseDTO
    {
        public string PostId { get; set; }
        public string AuthorId { get; set; }
        public string Content { get; set; }
        public Privacy Privacy { get; set; }
        public List<string> MediaUrls { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
        public bool IsEdited { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
    }
} 