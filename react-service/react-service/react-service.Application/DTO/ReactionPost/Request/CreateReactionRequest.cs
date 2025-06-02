using react_service.Domain.Entites;

namespace react_service.Application.DTO.ReactionPost.Request
{
    
    public class CreateReactionRequest
    {
        public string? PostId { get; set; }  
        public ReactionType ReactionType { get; set; } = ReactionType.Like;
    }
}
