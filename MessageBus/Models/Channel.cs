namespace MessageBus.Models
{
    public class Channel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Subscriber> Subscribers { get; set; }
    }
}
