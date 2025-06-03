namespace Domain.Entities
{
    public enum AttachmentType
    {
        Image = 1,
        Video ,
        Audio ,
        Document 
    }
    public class Attachment
    {
        public AttachmentType Type { get; set; }
        public string PublicId { get; set; } // for signed urls
    }
}
