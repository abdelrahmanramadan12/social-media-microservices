using react_service.Domain.Enums;

namespace react_service.Application.DTO.RabbitMQ
{
    public class PostReactionEventDTO
    {
        public string PostId { get; set; }
        public string UserId { get; set; }
        public ReactionEventType EventType { get; set; }
    }
}