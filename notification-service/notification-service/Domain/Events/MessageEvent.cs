namespace Domain.Events
{
    public class MessageEvent
    {
        public string? MessageId { get; set; }
        public string? SenderId { get; set; }
        public string? ReceiverId { get; set; }
        public string? Content { get; set; }
        public DateTime Timestamp { get; set; }
        //public MessageEvent(string? messageId, string? senderId, string? receiverId, string? content, DateTime timestamp)
        //{
        //    MessageId = messageId;
        //    SenderId = senderId;
        //    ReceiverId = receiverId;
        //    Content = content;
        //    Timestamp = timestamp;
        //}
    }
}
