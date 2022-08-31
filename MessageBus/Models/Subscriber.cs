using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MessageBus.Models
{
    public class Subscriber
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Channel> Channels { get; set; }
        public ICollection<MessageSubscriber> Messages { get; set; }
    }
}
