namespace BackOfficeWeb.Models.MessageBus
{
    public class Channel
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? deletedAt { get; set; }
        public ICollection<object> subscribers { get; set; } = new List<object>();
        public ICollection<object> messages { get; set; } = new List<object>();
        public int subscribersCount { get; set; } = 0;
        public int messagesCount { get; set; } = 0;
    }
}
