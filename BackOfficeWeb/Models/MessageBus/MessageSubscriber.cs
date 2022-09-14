namespace BackOfficeWeb.Models.MessageBus
{
    public class MessageSubscriber
    {
        public int messageId { get; set; }
        public int subscriberId { get; set; }
        public Message message { get; set; } = new Message();
        public Subscriber subscriber { get; set; } = new Subscriber();
        public DateTime? sentAt { get; set; }
    }
}
