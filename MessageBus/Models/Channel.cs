namespace MessageBus.Models
{
    public class Channel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; } = null;
        public ICollection<Subscriber> Subscribers { get; set; } = new List<Subscriber>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
