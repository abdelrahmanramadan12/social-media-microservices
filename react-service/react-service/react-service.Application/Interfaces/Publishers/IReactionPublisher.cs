using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using react_service.Application.DTO.RabbitMQ;
using react_service.Domain.Events;

namespace react_service.Application.Interfaces.Publishers
{
    public interface IReactionPublisher
    {
        Task PublishReactionAsync(PostReactionEventDTO reactionEvent);
        Task PublishReactionNotifAsync(ReactionEvent reactionEvent);

    }
}
