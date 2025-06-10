using react_service.Domain.Entites;
using react_service.Domain.Enums;

namespace react_service.Application.DTO.Reaction.Request.Post
{
    
    public class CreatePostReactionRequest
    {
        public string? PostId { get; set; }  
        public ReactionType ReactionType { get; set; } = ReactionType.Like;
    }
}
