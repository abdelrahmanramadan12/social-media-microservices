using react_service.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Application.DTO.RabbitMQ
{
    public class CommentReactionEventDTO
    {
        public string CommentId { get; set; }
        public string UserId { get; set; }
        public ReactionEventType EventType { get; set; }
    }
}
