namespace BackOfficeWeb.Models.MessageBus
{
    public class Message
    {
        public int id { get; set; }
        public string publisherName { get; set; }
        public DateTime createdAt { get; set; }
        public string data { get; set; }
        public Channel channel { get; set; } = new Channel();
        public ICollection<MessageSubscriber> subscribers { get; set; } = new List<MessageSubscriber>();
        public int subscribersCount { get; set; } = 0;
    }
}
