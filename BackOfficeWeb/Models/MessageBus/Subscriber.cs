namespace BackOfficeWeb.Models.MessageBus
{
    public class Subscriber
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? deletedAt { get; set; }
        public ICollection<Channel> channels { get; set; } = new List<Channel>();
        public ICollection<MessageSubscriber> messages { get; set; } = new List<MessageSubscriber>();
        public int channelsCount { get; set; } = 0;
        public int messagesCount { get; set; } = 0;
    }
}
