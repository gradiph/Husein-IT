using System.Text.Json.Serialization;

namespace MessageBus.Models
{
    public class Subscriber
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        [JsonIgnore]
        public virtual Channel Channel { get; set; }
        public int ChannelId { get; set; }
        [JsonIgnore]
        public virtual ICollection<MessageSubscriber> Messages { get; set; }
    }
}
