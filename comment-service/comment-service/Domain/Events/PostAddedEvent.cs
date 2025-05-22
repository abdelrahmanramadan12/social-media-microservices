using Domain.Entities;

namespace Domain.Events
{
    public class PostAddedEvent
    {
        public string PostId { get; set; }
        public string PostAuthorId { get; set; }
        public Privacy Privacy { get; set; }
    }
}
