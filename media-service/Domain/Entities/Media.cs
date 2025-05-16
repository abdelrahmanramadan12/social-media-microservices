using Domain.Enums;

namespace Domain.Entities
{
    public abstract class Media
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Url { get; set; } = string.Empty;
        public MediaType Type { get; set; }
        public UsageCategory UsageCategory { get; set; }
        public int Size { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
