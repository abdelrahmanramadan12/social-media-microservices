using Application.DTOs;
using Domain.Enums;

namespace Application.Events
{
    public class CommentEvent : QueueEvent
    {
        public string Id { get; set; }
        public string PostId { get; set; } = default!;
        public string AuthorId { get; set; } = default!;
        public string? Content { get; set; }
        public string? MediaUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 