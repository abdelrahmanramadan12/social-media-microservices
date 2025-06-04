using Domain.Entities;

namespace Service.Events
{
    public class PostData
    {
        public string PostId { get; set; }
        public string PostAuthorId { get; set; }
        public Privacy Privacy { get; set; }
    }
}
