namespace MessageBus.Models
{
    public class Channel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<Subscriber> Subscribers { get; set; }
    }
}
