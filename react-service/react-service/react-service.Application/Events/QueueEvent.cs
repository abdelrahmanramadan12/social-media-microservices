using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Application.Events
{
    public enum EventType
    {
        Create,
        Update,
        Delete,
        Follow,
        Unfollow
    }
    public class QueueEvent
    {
        public EventType EventType { get; set; }
    }
}
