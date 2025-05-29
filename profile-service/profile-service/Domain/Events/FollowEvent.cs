using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events
{
    public enum FollowEventType
    {
        Followed,
        Unfollowed
    }
    public class FollowEvent
    {
        public FollowEventType EventType { get; set; }
        public string FollowerId { get; set; }
        public string FollowedId { get; set; }

    }
}
