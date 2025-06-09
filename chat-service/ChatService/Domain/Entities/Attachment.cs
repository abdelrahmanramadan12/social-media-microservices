using Domain.Enums;

namespace Domain.Entities
{
    public class Attachment
    {
        public MediaType Type { get; set; }
        public string Url { get; set; } // for signed urls
    }
}
