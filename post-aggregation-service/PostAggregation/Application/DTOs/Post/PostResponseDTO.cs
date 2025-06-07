using System;
using System.Collections.Generic;

namespace Application.DTOs.Post
{
    public enum MediaType
    {
        Unknown,
        Image,
        Video,
        Audio,
        Document
    }
    public enum Privacy
    {
        Public,
        Private,
        OnlyMe
    }
    public class MediaDTO
    {
        public string Url { get; set; }
        public MediaType Type { get; set; }
    }
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