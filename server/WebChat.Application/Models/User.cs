namespace WebChat.Application.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
