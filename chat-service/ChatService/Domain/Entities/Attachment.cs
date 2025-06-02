namespace Domain.Entities
{
    public class Attachment
    {
        public string Type { get; set; }
        public string PublicId { get; set; } // for signed urls
    }
}
