using react_service.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reat_service.Application.DTO.RabbitMQ
{
    public  class ReactionPublishDTO    
    {
        public string PostId { get; set; }
        public string ReactorId { get; set; }

        public EventType EventType { get; set; }       
    }
}
