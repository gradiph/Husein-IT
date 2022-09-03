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
        public DateTime? DeletedAt { get; set; } = null;

        public ICollection<Channel> Channels { get; set; } = new List<Channel>();
        public ICollection<MessageSubscriber> Messages { get; set; } = new List<MessageSubscriber>();
    }
}
