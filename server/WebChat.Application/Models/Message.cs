namespace WebChat.Application.Models
{
    public class Message
    {
        public Guid RoomId { get; set; }
        public DateTimeOffset SentAt { get; set; } = DateTimeOffset.UtcNow;
        public Guid MessageId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; }
    }
}