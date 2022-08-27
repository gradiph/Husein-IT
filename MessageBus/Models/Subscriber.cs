using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MessageBus.Models
{
    public class Subscriber
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public ICollection<Channel> Channels { get; set; }
        public ICollection<MessageSubscriber> Messages { get; set; }
    }
}
