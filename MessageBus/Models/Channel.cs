namespace MessageBus.Models
{
    public class Channel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Subscriber> Subscribers { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
